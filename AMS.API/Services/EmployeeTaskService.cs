using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectOversight.API.Constants;
using ProjectOversight.API.Data;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services.Interface;
using Task = ProjectOversight.API.Data.Model.Task;

namespace ProjectOversight.API.Services
{
    public class EmployeeTaskService : IEmployeeTaskService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ProjectOversightContext _dbContext;


        public EmployeeTaskService(IUnitOfWork repository, IMapper mapper, UserManager<User> userManager,
            RoleManager<Role> roleManager, IConfiguration configuration, ProjectOversightContext dbContext)
        {
            _mapper = mapper;
            _userManager = userManager;
            _repository = repository;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<List<Task>> GetProjectTasklist(int Id)
        {
            try
            {
                var projectList = await _repository.Task.FindByConditionAsync(x => x.ProjectId == Id);
                var result = projectList.ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Task> GetProjectTaskById(int Id)
        {
            try
            {
                var projectList = await _repository.Task.FindById(x => x.Id == Id);
                return projectList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<EmployeeTask> AssignEmployeeTask(User user, EmployeeTaskDto dayPlan)
        {
            try
            {
                DateTime currentDate = DateTime.Today;
              /*  DateTime weekEndingDate;
                int daysUntilFriday = (int)DayOfWeek.Friday - (int)currentDate.DayOfWeek;
                if (daysUntilFriday <= 0)
                    daysUntilFriday += 7;
                weekEndingDate = currentDate.AddDays(daysUntilFriday);
                weekEndingDate = weekEndingDate.Date.AddDays(1).AddTicks(-1);*/
                var EmployeeDayPlan = _mapper.Map<EmployeeTask>(dayPlan);
                var day = await _repository.day.FindById(x => x.Date.Date == currentDate.Date);

                EmployeeDayPlan.Status = Status.InProgress;
                EmployeeDayPlan.CreatedBy = user.Id.ToString();
                EmployeeDayPlan.UpdatedBy = user.Id.ToString();
                EmployeeDayPlan.StartDate = DateTime.Now;
                EmployeeDayPlan.CreatedDate = DateTime.Now;
                EmployeeDayPlan.WeekEndingDate = dayPlan.WeekEndingDate;
                EmployeeDayPlan.DayId = day.Id;

                var taskcreated = await _repository.EmployeeTask.CreateAsync(EmployeeDayPlan);
                var taskList = await _repository.Task.FindByConditionAsync(x => x.Id == taskcreated.TaskId);
                var taskListDetails = taskList.First();
                taskListDetails.Status = Status.Assigned;
                var Updatetask = await _repository.Task.UpdateAsync(taskListDetails);
                Comments comment = new()
                {
                    ProjectId = dayPlan.ProjectId,
                    TaskId = dayPlan.TaskId,
                    EmployeeId = dayPlan.EmployeeId,
                    EmployeeTaskId = taskcreated.Id,
                    CreatedBy = user.Id.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedBy = user.Id.ToString(),
                    Comment = dayPlan.Comment,
                };
                var addComments = await _repository.Comments.CreateAsync(comment);


                return taskcreated;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> GetEmployeeTaskbyId(int EmployeeId, int TaskId)
        {
            try
            {
                var projectList = await _repository.EmployeeTask.FindByConditionAsync(x => x.EmployeeId == EmployeeId && x.TaskId == TaskId);
                var result = projectList.Any(); 

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<CommentsDto>> GetComments()
        {
            try
            {
                var commentList = (from comment in _dbContext.Comments
                             join project in _dbContext.Project on comment.ProjectId equals project.Id
                             join employee in _dbContext.Employee on comment.EmployeeId equals employee.Id
                             join user in _dbContext.Users on employee.UserId equals user.Id
                             where comment.EmployeeTaskId != null
                             select new CommentsDto()
                             {
                                 Id = comment.Id,
                                 Comment = comment.Comment,
                                 Project = project.Name,
                                 Employee = user.Name,
                                 EmployeeTaskId = comment.EmployeeTaskId
                             }).ToList();
            return commentList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Task> GetTaskDetalisById(int TaskId)
        {
            try
            {
                var TaskDetalisList = await _repository.Task.FindById(x => x.Id == TaskId);
                return TaskDetalisList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<EmployeeTask> GetAssignedEmployeeTaskById(int TaskId, int projectId)
        {
            try
            {
                var TaskDetalisList = await _repository.EmployeeTask.FindById(x => x.TaskId == TaskId && x.ProjectId == projectId);
                return TaskDetalisList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<EmployeeTaskDto>> GetWhatsapptaskListByTaskId(string Status, int employeeId, DateTime WeekEndingDate)
        {
            try
            {
                var taskDetailsList = await _repository.EmployeeTask.FindByConditionAsync(x => x.Status == Status && x.EmployeeId == employeeId && x.WeekEndingDate == WeekEndingDate);
                var taskIdList = taskDetailsList.Select(x => x.TaskId).ToList();
                var taskDetails = await _repository.Task.FindByConditionAsync(t => taskIdList.Contains(t.Id));

                var taskDetailsWithNamesAndDescriptions = taskDetailsList
                    .Join(taskDetails,
                        empTask => empTask.TaskId,
                        task => task.Id,
                        (empTask, task) => new EmployeeTaskDto
                        {
                            Id = empTask.Id,
                            Name = task.Name,
                            TaskDescription = task.TaskDescription,
                            EstTime = empTask.EstTime,
                            ProjectId = task.ProjectId,
                        })
                    .ToList();

                var projectIds = taskDetailsWithNamesAndDescriptions.Select(x => x.ProjectId).ToList();
                var projects = await _repository.Project.FindByConditionAsync(p => projectIds.Contains(p.Id));

                foreach (var taskDetail in taskDetailsWithNamesAndDescriptions)
                {
                    var project = projects.FirstOrDefault(p => p.Id == taskDetail.ProjectId);
                    if (project != null)
                    {
                        taskDetail.ProjectName  =project.Name ;
                    }
                }

                return taskDetailsWithNamesAndDescriptions;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
