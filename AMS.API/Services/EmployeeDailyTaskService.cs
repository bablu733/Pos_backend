using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectOversight.API.Constants;
using ProjectOversight.API.Data;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services.Interface;
using System.Xml.Linq;

namespace ProjectOversight.API.Services
{
    public class EmployeeDailyTaskService : IEmployeeDailyTaskService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ProjectOversightContext _dbContext;
        public EmployeeDailyTaskService(IUnitOfWork repository, IMapper mapper, UserManager<User> userManager,
            RoleManager<Role> roleManager, IConfiguration configuration, ProjectOversightContext dbContext)
        {
            _mapper = mapper;
            _userManager = userManager;
            _repository = repository;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }
        public async Task<List<EmployeeDailyTaskDto>> GetTimePlanList()
        {
            try
            {
                var timePlanList = (from employeeDailyTask in _dbContext.EmployeeDailyTask
                             join user in _dbContext.Users on employeeDailyTask.EmployeeId equals user.Id
                             join comment in _dbContext.Comments on employeeDailyTask.Id equals comment.EmployeeDailyTaskId into commentGroup
                             from comment in commentGroup.DefaultIfEmpty()
                             join project in _dbContext.Project on employeeDailyTask.ProjectId equals project.Id into projectGroup
                             from project in projectGroup.DefaultIfEmpty()
                             join employeeTask in _dbContext.EmployeeTask on employeeDailyTask.EmployeeTaskId equals employeeTask.Id into employeeTaskGroup
                             from employeeTask in employeeTaskGroup.DefaultIfEmpty()
                             join task in _dbContext.Task on employeeTask.TaskId equals task.Id into taskGroup
                             from task in taskGroup.DefaultIfEmpty()
                             select new EmployeeDailyTaskDto()
                             {
                                 Id=employeeDailyTask.Id,
                                 Name = employeeDailyTask.Name,
                                 Description = employeeDailyTask.Description,
                                 EmployeeName = user.Name,
                                 ProjectName = project.Name,
                                 StartDate = employeeDailyTask.StartDate,
                                 EndDate = employeeDailyTask.EndDate,
                                 EstTime = employeeDailyTask.EstTime,
                                 ActTime = employeeDailyTask.ActTime,
                                 WeekEndingDate = employeeDailyTask.WeekEndingDate,
                                 Status = employeeDailyTask.Status,
                                 Priority = employeeDailyTask.Priority,
                                 Percentage = employeeDailyTask.Percentage
                             }).OrderByDescending(x => x.Id).ToList();
                return timePlanList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<EmployeeTaskListDto>> GetEmployeeTask(int EmployeeId)
        {
            try
            {
                var employeeTaskList = await _repository.EmployeeTask.FindByConditionAsync(x => x.EmployeeId == EmployeeId);
                var taskIds = employeeTaskList.Select(et => et.TaskId).ToList();
                var taskList = await _repository.Task.FindByConditionAsync(t => taskIds.Contains(t.Id));

                var result = employeeTaskList
                    .Join(taskList,
                        et => et.TaskId,
                        t => t.Id,
                        (et, t) => new EmployeeTaskListDto
                        {
                            Id = et.Id,
                            EmployeeId = et.EmployeeId,
                            Status = et.Status,
                            TaskId = t.Id,
                            Name = t.Name,
                            Description = t.Description,
                            EstTime = et.EstTime,
                            EstStartDate = et.EstStartDate,
                            ProjectId = et.ProjectId,
                            EstEndDate = et.EstEndDate,
                            Percentage = et.Percentage
                        })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<EmployeeDailyTask> AddEmployeeDailyTask(User user, EmployeeDailyTaskDto employeeDailyTask)
        {
            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime weekEndingDate;
                int daysUntilFriday = (int)DayOfWeek.Friday - (int)currentDate.DayOfWeek;
                if (daysUntilFriday <= 0)
                    daysUntilFriday += 7;
                weekEndingDate = currentDate.AddDays(daysUntilFriday);
                weekEndingDate = weekEndingDate.Date.AddDays(1).AddTicks(-1);
                var employeeDaily = _mapper.Map<EmployeeDailyTask>(employeeDailyTask);
                var task = await _repository.Task.FindById(x => x.Id == employeeDailyTask.TaskId);
                var category = await _repository.Category.FindById(x => x.Id == task.CategoryId);
                UserStoryUI userStoryUI = null;
                if (task.UserStoryUIId == 0)
                {
                    userStoryUI = null;
                }
                else
                {
                    userStoryUI = await _repository.UserStoryUI.FindById(x => x.Id == task.UserStoryUIId);
                }
                var projectUS = await _repository.UserStory.FindByConditionAsync(x => x.ProjectId == task.ProjectId);
                var project = await _repository.Project.FindById(x => x.Id == task.ProjectId);
                UserInterface ui = new();
                IEnumerable<UserStory> us = new List<UserStory>();
                if (userStoryUI != null)
                {
                    ui = await _repository.UserInterface.FindById(x => x.Id == userStoryUI.UIId);
                    us = await _repository.UserStory.FindByConditionAsync(x => x.Id == userStoryUI.UserStoryId);
                }
                var employeeTask = await _repository.EmployeeTask.FindById(x => x.Id == employeeDailyTask.EmployeeTaskId);
                {
                    employeeDaily.WeekEndingDate = weekEndingDate;
                    employeeDaily.StartDate = employeeTask.StartDate;
                    employeeDaily.EndDate = currentDate;
                    employeeDaily.ActTime = employeeDailyTask.ActTime;
                    employeeDaily.EstTime = employeeTask.EstTime;
                    employeeDaily.Name = employeeDailyTask.Name;
                    employeeDaily.Percentage = employeeDailyTask.Percentage;
                    employeeDaily.Description = employeeDailyTask.Description;
                    employeeDaily.CreatedDate = DateTime.Now;
                    employeeDaily.CreatedBy = user.Id.ToString();
                    employeeDaily.UpdatedDate = DateTime.Now;
                    employeeDaily.UpdatedDate = DateTime.Now;
                    employeeDaily.UpdatedBy = user.Id.ToString();
                    employeeDaily.WorkedOn = DateTime.Now;
                    if (employeeDaily.Percentage == 100 && category.Categories == "Development" && task.Category.SubCategory == "Development")
                    {
                        employeeDaily.Status = "Ready-For-UAT";
                    }
                    else if(employeeDaily.Percentage == 100)
                    {
                        employeeDaily.Status = "Completed";
                    }
                    else
                    {
                        employeeDaily.Status = "In-Progress";
                    }
                }
                var dailyTaskcreated = await _repository.DailyTask.CreateAsync(employeeDaily);
                {
                    employeeTask.Percentage = employeeDailyTask.Percentage;
                    employeeTask.UpdatedDate = DateTime.Now;
                    employeeTask.UpdatedBy = user.Id.ToString();
                    if(employeeTask.Percentage == 100 && category.Categories == "Development" && category.SubCategory == "Development")
                    {
                        employeeTask.Status = "Ready-For-UAT";
                    }
                    else if (employeeTask.Percentage == 100)
                    {
                        employeeTask.Status = "Completed";
                    }
                    else
                    {
                        employeeTask.Status = "In-Progress";
                    }
                }
                var updateEmployeeTask = await _repository.EmployeeTask.UpdateAsync(employeeTask);
                {
                    task.Percentage = employeeDailyTask.Percentage;
                    task.UpdatedDate = DateTime.Now;
                    task.UpdatedBy = user.Id.ToString();
                    if(task.Percentage == 100 && category.Categories == "Development" && category.SubCategory == "Development")
                    {
                        task.Status = "Ready-For-UAT";
                    }
                    else if (task.Percentage == 100)
                    {
                        task.Status = "Completed";
                    }
                    else
                    {
                        task.Status = "In-Progress";
                    }
                }
                var updateTask = await _repository.Task.UpdateAsync(task);
                {
                    ui.Percentage = employeeDailyTask.Percentage;
                    ui.UpdatedDate = DateTime.Now;
                    ui.UpdatedBy = user.Id.ToString();
                }
                if (ui.Id != 0)
                {
                    var updateUI = await _repository.UserInterface.UpdateAsync(ui);
                }
                if (us.Count() != 0)
                {
                    int computedPercentage;
                    computedPercentage = employeeDailyTask.Percentage / us.Count();
                    foreach (UserStory story in us)
                    {
                        story.Percentage = computedPercentage;
                        story.UpdatedDate = DateTime.Now;
                        story.UpdatedBy = user.Id.ToString();
                        await _repository.UserStory.UpdateAsync(story);
                    }

                }
                Comments comment = new()
                {
                    ProjectId = employeeTask.ProjectId,
                    TaskId = employeeTask.TaskId,
                    EmployeeId = employeeTask.EmployeeId,
                    EmployeeTaskId = employeeTask.Id,
                    EmployeeDailyTaskId = dailyTaskcreated.Id,
                    CreatedBy = user.Id.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedBy = user.Id.ToString(),
                    Comment = employeeDailyTask.Comment,
                };
                var addComments = await _repository.Comments.CreateAsync(comment);

                //if(projectUS.Count() != 0)
                //{
                //    int computedPercentage;
                //    computedPercentage = employeeDailyTask.Percentage / projectUS.Count();
                //    project.Percentage = computedPercentage;
                //    project.UpdatedDate = DateTime.Now;
                //    project.UpdatedBy = user.Id.ToString();
                //    await _repository.Project.UpdateAsync(project);
                //}
                return employeeDaily;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<EmployeeDailyTaskDto>> GetEmployeeTimePlanList(int employeeId)
        {
            try
            {
                var timePlanList = (from employeeDailyTask in _dbContext.EmployeeDailyTask
                                    join user in _dbContext.Users on employeeDailyTask.EmployeeId equals user.Id
                                    join comment in _dbContext.Comments on employeeDailyTask.Id equals comment.TaskId into commentGroup
                                    from comment in commentGroup.DefaultIfEmpty()
                                    join project in _dbContext.Project on employeeDailyTask.ProjectId equals project.Id into projectGroup
                                    from project in projectGroup.DefaultIfEmpty()
                                    join employeeTask in _dbContext.EmployeeTask on employeeDailyTask.EmployeeTaskId equals employeeTask.Id into employeeTaskGroup
                                    from employeeTask in employeeTaskGroup.DefaultIfEmpty()
                                    join task in _dbContext.Task on employeeTask.TaskId equals task.Id into taskGroup
                                    from task in taskGroup.DefaultIfEmpty()
                                    where employeeDailyTask.EmployeeId == employeeId
                                    select new EmployeeDailyTaskDto()
                                    {
                                        Id = employeeDailyTask.Id,
                                        Name = employeeDailyTask.Name,
                                        Description = employeeDailyTask.Description,
                                        EmployeeName = user.Name,
                                        ProjectName = project.Name,
                                        StartDate = employeeDailyTask.StartDate,
                                        EndDate = employeeDailyTask.EndDate,
                                        EstTime = employeeDailyTask.EstTime,
                                        ActTime = employeeDailyTask.ActTime,
                                        WeekEndingDate = employeeDailyTask.WeekEndingDate,
                                        Status = employeeDailyTask.Status,
                                        Priority = employeeDailyTask.Priority,
                                        Percentage = employeeDailyTask.Percentage
                                    }).ToList();
                return timePlanList;
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
                             where comment.EmployeeDailyTaskId != null
                             select new CommentsDto()
                             {
                                 Id = comment.Id,
                                 Comment = comment.Comment,
                                 Project=project.Name,
                                 Employee = user.Name,
                                 EmployeeDailyTaskId = comment.EmployeeDailyTaskId
                             }).ToList();
                return commentList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<EmployeeDailyTask> GetEmployeeDailyTaskById(int employeeId, int projectId)
        {
            try
            {
                var TaskDetalisList = await _repository.DailyTask.FindByConditionAsync(x => x.EmployeeId == employeeId && x.ProjectId == projectId);
                return TaskDetalisList.LastOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<EmployeeDailyTask>> GetEmployeeDailyTask(int employeeTaskId)
        {
            var employeeDailyTasks = (from employeeDailyTask in _dbContext.EmployeeDailyTask
                                      join user in _dbContext.Users on employeeDailyTask.CreatedBy equals user.Id.ToString()
                                      where employeeDailyTask.EmployeeTaskId == employeeTaskId
                                      select new EmployeeDailyTask() 
                                      {
                                        Name = employeeDailyTask.Name,
                                        Description = employeeDailyTask.Description,
                                        EmployeeTaskId = employeeDailyTask.EmployeeTaskId,
                                        CreatedBy = user.Name,
                                        Percentage = employeeDailyTask.Percentage,
                                        StartDate = employeeDailyTask.StartDate,
                                        EndDate = employeeDailyTask.EndDate,
                                        Status = employeeDailyTask.Status,
                                        ActTime = employeeDailyTask.ActTime,
                                        EstTime = employeeDailyTask.EstTime
                                      }).OrderBy(x => x.Percentage).ToList();
            return employeeDailyTasks;
        }

        public async Task<List<EmployeeTaskDto>> GetCompletedWhatsapptaskListByTaskId(int employeeId, DateTime WeekEndingDate)
        {
            try
            {
                var taskDetailsList = await _repository.EmployeeTask.FindByConditionAsync(x => x.EmployeeId == employeeId && x.WeekEndingDate == WeekEndingDate);
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
                            Status = empTask.Status,
                        })
                    .ToList();

                var projectIds = taskDetailsWithNamesAndDescriptions.Select(x => x.ProjectId).ToList();
                var projects = await _repository.Project.FindByConditionAsync(p => projectIds.Contains(p.Id));

                foreach (var taskDetail in taskDetailsWithNamesAndDescriptions)
                {
                    var project = projects.FirstOrDefault(p => p.Id == taskDetail.ProjectId);
                    if (project != null)
                    {
                        taskDetail.ProjectName = project.Name;
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
