using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectOversight.API.Data;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services.Interface;

namespace ProjectOversight.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ProjectOversightContext _dbContext;

        public EmployeeService(IUnitOfWork repository, IMapper mapper, UserManager<User> userManager,
            RoleManager<Role> roleManager, IConfiguration configuration, ProjectOversightContext dbContext)
        {
            _mapper = mapper;
            _userManager = userManager;
            _repository = repository;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<List<EmployeeDailyTask>> GetEmployeeHistory(DateTime fromDate, DateTime toDate)
        {
            try
            {
                //var employee = await _repository.Employee.FindById(x => x.UserId == user.Id);
                var employeeHistory = await _repository.DailyTask.FindByConditionAsync(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate);
                var result = employeeHistory.ToList();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateEmployee(UserCreateDto userCreateDto)
        {
            try
            {
                var employee = await _repository.Employee.FindById(x => x.UserId == userCreateDto.id);
                employee.Name = userCreateDto.UserName;
                employee.PhoneNumber = userCreateDto.PhoneNumber;
                employee.Department = userCreateDto.Department;
                employee.Category = userCreateDto.Category;
                employee.UpdatedDate = DateTime.Now;
                await _repository.Employee.UpdateAsync(employee);

                var user = await _repository.User.FindById(x => x.Id == userCreateDto.id);
                user.PhoneNumber = userCreateDto.PhoneNumber;
                user.Email = userCreateDto.Email;
                user.SecondaryEmail = userCreateDto.SecondaryEmail;
                user.SecondaryPhoneNumber = userCreateDto.SecondaryPhoneNumber;
                user.Name = userCreateDto.UserName;
                await _repository.User.UpdateAsync(user);

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<List<Employee>> GetEmployeeList()
        {
            try
            {
                var result = await _dbContext.Set<Employee>().Include(e => e.User).OrderByDescending(x => x.Id).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> AssignSkill(EmployeeSkillSet[] empSkillSet)
        {
            try
            {
                var skillSet = _dbContext.EmployeeSkillSet.Where(x => x.EmployeeId == empSkillSet[0].EmployeeId).ToList();
                _dbContext.EmployeeSkillSet.RemoveRange(skillSet);

                foreach (var obj in empSkillSet)
                {

                    EmployeeSkillSet employeeSkillSet = new EmployeeSkillSet()
                    {
                        SkillSetId = obj.SkillSetId,
                        EmployeeId = obj.EmployeeId,
                        CreatedBy = obj.CreatedBy,
                        UpdatedBy = obj.UpdatedBy,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };
                    await _dbContext.EmployeeSkillSet.AddAsync(employeeSkillSet);


                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<List<EmployeeSkillSet>> GetEmployeeSkillById(int employeeId)
        {

            try
            {
                var result = await _dbContext.EmployeeSkillSet.Where(x => x.EmployeeId == employeeId).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<EmployeeStatDto> GetEmployeeStatDetails(User user)
        {
            try
            {
                var employee = await _repository.Employee.FindById(x => x.UserId == user.Id);
                var employeeProject = await _repository.EmployeeProject.GetEmployeeProjectList(employee.Id);
                var totalTask = await _repository.EmployeeTask.FindByConditionAsync(x => x.EmployeeId == employee.Id);
                var inProgressTask = await _repository.EmployeeTask.FindByConditionAsync(x => x.EmployeeId == employee.Id && x.Status == "In-Progress");
                var completedTask = await _repository.EmployeeTask.FindByConditionAsync(x => x.EmployeeId == employee.Id && (x.Status == "Completed" || x.Status == "Ready-For-UAT"));
                var result = inProgressTask.ToList();
                EmployeeStatDto empStat = new()
                {
                    TotalProject = employeeProject.Count(),
                    InProgressTask = inProgressTask.Count(),
                    TotalTask = totalTask.Count(),
                    CompletedTask = completedTask.Count()
                };
                return empStat;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TeamDto>> GetEmployeeTask(int teamId, DateTime? weekend)
        {
            try
            {
                var query = await (from team in _dbContext.Team
                                   join teamEmployee in _dbContext.TeamEmployee on team.Id equals teamEmployee.TeamId
                                   join employee in _dbContext.Employee on teamEmployee.EmployeeId equals employee.Id
                                   join user in _dbContext.Users on employee.UserId equals user.Id
                                   where team.Id == teamId && teamEmployee.EndDate == null
                                   select new TeamDto
                                   {
                                       TeamId = team.Id,
                                       TeamName = team.Name,
                                       EmployeeId = teamEmployee.EmployeeId,
                                       EmployeeName = user.Name,
                                   }
                    ).Distinct().ToListAsync();

                DateTime weekEnd = weekend == null ? WeekEndingDate() : (DateTime)weekend;
                DateTime? weekStart = weekEnd.AddDays(-5);

                foreach (var obj in query)
                {
                    var empTask = _dbContext.EmployeeTask.Where(x => x.EmployeeId == obj.EmployeeId && x.WeekEndingDate.Date == weekEnd.Date).ToList();
                    decimal total = 0;
                    foreach (var emp in empTask)
                    {
                        if (emp.WeekEndingDate.Date == weekEnd.Date)
                        {
                            total += emp.EstTime;
                        }
                    }
                    obj.AssignedHours = total;
                }
                return query;
            } catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<EmployeeProject>> GetEmployeeProject(int projectId)
        {
            try
            {
                var result = await _dbContext.EmployeeProject.Where(x => x.ProjectId == projectId && x.EndDate == null).ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<AttendanceDto> GetEmployeeAttendance(DateTime selectedDate)
        {
            try
            {
                var day = _dbContext.Day.FirstOrDefault(x => x.Date.Date == selectedDate.Date);
                DateTime OfficeTime = new DateTime(1, 1, 1, 10, 10, 0);

                var query = await (from employee in _dbContext.Employee
                                   join user in _dbContext.Users on employee.UserId equals user.Id into userGroup
                                   from user in userGroup.DefaultIfEmpty()
                                   select new AttendenceVm()
                                   {
                                       Id = user.Id,
                                       DayId = day.Id,
                                       EmployeeName = user.Name,
                                       Department = employee.Department,
                                       Date = day.Date,
                                       InTime = _dbContext.EmployeeTime.FirstOrDefault(x => x.DayId == day.Id && x.EmployeeId == employee.Id).InTime,
                                       OutTime = _dbContext.EmployeeTime.OrderBy(x => x.CreatedDate).LastOrDefault(x => x.DayId == day.Id && x.EmployeeId == employee.Id).OutTime,
                                       EmployeeTime = _dbContext.EmployeeTime.Where(x => x.DayId == day.Id && x.EmployeeId == employee.Id).ToList(),
                                       InOutCount = _dbContext.EmployeeTime.Where(x => x.DayId == day.Id && x.EmployeeId == employee.Id).Count(),
                                       TotalWorkHour = 0,
                                   }).OrderBy(x => x.EmployeeName).ToListAsync();

                var totalEmployees = _dbContext.Users.Count();
                var presentEmployees = _dbContext.EmployeeDay.Where(x => x.DayId == day.Id).Count();
                var absent = totalEmployees - presentEmployees;
                int onTime = 0;
                DateTime date;

                foreach (var obj in query)
                {
                    if (obj.InTime != null)
                    {
                        date = (DateTime)obj.InTime;

                        if (date.TimeOfDay <= OfficeTime.TimeOfDay)
                        {
                            onTime++;
                        }
                    }
                }

                var attendence = new AttendanceDto()
                {
                    EmployeeCount = totalEmployees,
                    Present = presentEmployees,
                    Absent = absent,
                    onTime = onTime,
                    Late = presentEmployees - onTime,
                    Attendances = query
                };

                return attendence;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<EmployeeAttendanceVM> GetAttendanceByEmployeeId(int userId)
        {
            try
            {
                TimeSpan? averageInTime = null;
                TimeSpan? averageOutTime = null;
                var EmployeeTime = await _dbContext.EmployeeTime.ToListAsync();
                var Day = await _dbContext.Day.ToListAsync();
                int dayId = Day.FirstOrDefault(x => x.Date.Date == DateTime.Now.Date).Id;
                var employee = _dbContext.Employee.FirstOrDefault(x => x.UserId == userId);
                DateTime employeeCreatedDate = (DateTime)employee.CreatedDate;
                int startDayId = Day.FirstOrDefault(x => x.Date.Date == employeeCreatedDate.Date).Id;
                int totalWorkDays = Day.Where(x => x.Id >= startDayId && x.Id <= dayId).Count();
                int totalAttendance = _dbContext.EmployeeDay.Where(x => x.EmployeeId == employee.Id && x.DayId >= startDayId).Count();
                int totalAbsent = totalWorkDays - totalAttendance;

                var lastThreeInTime = EmployeeTime.Where(x => x.EmployeeId == employee.Id && x.DayId != null && x.OutTime != null && x.InTime != null).OrderByDescending(x => x.DayId).Take(5).ToList();
                if (lastThreeInTime.Count() >= 3)
                {
                    int? lastDayId = lastThreeInTime[0].DayId;
                    DateTime startDay1 = (DateTime)lastThreeInTime.Where(x => x.DayId == lastDayId).FirstOrDefault().InTime;
                    DateTime startDay2 = (DateTime)lastThreeInTime.Where(x => x.DayId == lastDayId - 1).FirstOrDefault().InTime;
                    DateTime startDay3 = (DateTime)lastThreeInTime.Where(x => x.DayId == lastDayId - 2).FirstOrDefault().InTime;

                    DateTime endDay1 = (DateTime)lastThreeInTime.Where(x => x.DayId == lastDayId).OrderBy(x => x.InTime).LastOrDefault().OutTime;
                    DateTime endDay2 = (DateTime)lastThreeInTime.Where(x => x.DayId == lastDayId - 1).OrderBy(x => x.InTime).LastOrDefault().OutTime;
                    DateTime endDay3 = (DateTime)lastThreeInTime.Where(x => x.DayId == lastDayId - 2).OrderBy(x => x.InTime).LastOrDefault().OutTime;

                    averageInTime = (startDay1.TimeOfDay + startDay2.TimeOfDay + startDay3.TimeOfDay) / 3;
                    averageOutTime = (endDay1.TimeOfDay + endDay2.TimeOfDay + endDay3.TimeOfDay) / 3;
                }

                var attendanceHistory = (from day in _dbContext.Day
                                         where day.Id >= startDayId && day.Id <= dayId
                                         select new EmployeeAttendanceDto()
                                         {
                                             Date = day.Date,
                                             InTime = _dbContext.EmployeeTime.FirstOrDefault(x => x.DayId == day.Id && x.EmployeeId == employee.Id).InTime,
                                             OutTime = _dbContext.EmployeeTime.Where(x => x.DayId == day.Id && x.EmployeeId == employee.Id).OrderBy(x => x.InTime).LastOrDefault().OutTime,
                                             Latitude = _dbContext.EmployeeGeo.FirstOrDefault(x => x.DayId == day.Id && x.EmployeeId == employee.Id).Latitude,
                                             Longitude = _dbContext.EmployeeGeo.FirstOrDefault(x => x.DayId == day.Id && x.EmployeeId == employee.Id).Longitude
                                         }).OrderByDescending(x => x.Date).ToList();

                var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);

                EmployeeAttendanceVM attendenceVm = new EmployeeAttendanceVM()
                {
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Role = "Employee",
                    TotalAttendance = totalAttendance,
                    TotalAbsent = totalAbsent,
                    AverageInTime = averageInTime?.ToString(@"hh\:mm"),
                    AverageOutTime = averageOutTime?.ToString(@"hh\:mm"),
                    EmployeeAttendances = attendanceHistory
                };

                return attendenceVm;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<EmployeeTaskVM> GetEmployeeTasks(int employeeId)
        {
            try
            {

            
            var employeeProjects = (from employeeProject in _dbContext.EmployeeProject
                                    join project in _dbContext.Project on employeeProject.ProjectId equals project.Id
                                    where employeeProject.EmployeeId == employeeId
                                    select new Project()
                                    {
                                        Id = project.Id,
                                        Name = project.Name
                                    }).ToList();

                var employeeTasks = (from employeeTask in _dbContext.EmployeeTask
                                     join task in _dbContext.Task on employeeTask.TaskId equals task.Id
                                     join user in _dbContext.Users on employeeTask.CreatedBy equals user.Id.ToString()
                                     where employeeTask.EmployeeId == employeeId
                                     select new EmployeeTaskDto()
                                     {
                                         Id =employeeTask.Id,
                                         Name = task.Name,
                                         Description = task.Description,
                                         Status = employeeTask.Status,
                                         StartDate = employeeTask.EstEndDate,
                                         EndDate = employeeTask.EstEndDate,
                                         WeekEndingDate = employeeTask.WeekEndingDate,
                                         CreatedBy = user.Name,
                                         ProjectId = employeeTask.ProjectId,
                                         EstTime = employeeTask.EstTime,
                                         Percentage = employeeTask.Percentage
                                     }).ToList();

                EmployeeTaskVM employeeTaskVM = new EmployeeTaskVM()
                {
                    EmployeeDailyTask = employeeTasks,
                    EmployeeProjects = employeeProjects
                };

                return employeeTaskVM;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DateTime WeekEndingDate()
        {
            DateTime currentDate = DateTime.Today;
            DateTime weekEndingDate;
            int daysUntilFriday = (int)DayOfWeek.Friday - (int)currentDate.DayOfWeek;
            if (daysUntilFriday <= 0)
                daysUntilFriday += 7;

            weekEndingDate = currentDate.AddDays(daysUntilFriday);
            weekEndingDate = weekEndingDate.Date.AddDays(1).AddTicks(-1);
            return weekEndingDate;
        }

        public async Task<Employee> UpdateByEmployeeId(int UserId, EmployeeDto employeeDto)
        {
            try
            {
                IFormFile profilePhotoFile = employeeDto.ProfilePhotoBase64;
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await profilePhotoFile.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                string base64String = Convert.ToBase64String(fileBytes);

                var employee = await _repository.Employee.FindById(x => x.UserId==UserId);
                employee.UserId=UserId;
                employee.Name = employeeDto.Name;
                employee.ProfilePhoto = base64String;
                employee.PhoneNumber= employeeDto.PhoneNumber;
            
                var taskemployee = new Employee
                {
                    UserId = employee.UserId,
                    Name = employeeDto.Name,
                    ProfilePhoto = base64String,
                    PhoneNumber = employeeDto.PhoneNumber

                };
            var employeeDetails = await _repository.Employee.UpdateAsync(taskemployee);

                return taskemployee;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
