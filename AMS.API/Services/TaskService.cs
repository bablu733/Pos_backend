using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectOversight.API.Data;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services.Interface;
using System.Threading.Tasks;
using System.Xml.Linq;
using Task = ProjectOversight.API.Data.Model.Task;

namespace ProjectOversight.API.Services
{
    public class TaskService : ITaskService
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;
        private readonly ProjectOversightContext _dbContext;
        public TaskService(IUnitOfWork repository, IMapper mapper, ProjectOversightContext dbContext)
        {
            _mapper = mapper;
            _repository = repository;
            _dbContext = dbContext;
        }
        public async Task<List<TaskListDto>> GetTaskList()
        {
            try
            {
                var taskList = (from task in _dbContext.Task
                                join project in _dbContext.Project on task.ProjectId equals project.Id into projectJoin
                                from project in projectJoin.DefaultIfEmpty()
                                join employeeTask in _dbContext.EmployeeTask on task.Id equals employeeTask.TaskId into employeeTaskJoin
                                from employeeTask in employeeTaskJoin.DefaultIfEmpty()
                                join team in _dbContext.Team on task.TeamId equals team.Id into teamJoin
                                from team in teamJoin.DefaultIfEmpty()
                                join employee in _dbContext.Employee on employeeTask.EmployeeId equals employee.Id into employeeJoin
                                from employee in employeeJoin.DefaultIfEmpty()
                                join user in _dbContext.Users on employee.UserId equals user.Id into userJoin
                                from user in userJoin.DefaultIfEmpty()
                                //join userStoryUI in _dbContext.UserStoryUI on task.UserStoryUIId equals userStoryUI.Id into userStoryUIJoin
                                //from userStoryUI in userStoryUIJoin.DefaultIfEmpty()
                                //join userStory in _dbContext.UserStory on userStoryUI.UserStoryId equals userStory.Id into userStoryJoin
                                //from userStory in userStoryJoin.DefaultIfEmpty()
                                //join userInterface in _dbContext.UserInterface on userStoryUI.UIId equals userInterface.Id into userInterfaceJoin
                                //from userInterface in userInterfaceJoin.DefaultIfEmpty()
                                select new TaskListDto()
                                {
                                    Id = task.Id,
                                    ProjectId = task.ProjectId,
                                    Name = task.Name,
                                    projectName = project.Name,
                                    Description = task.Description,
                                    TeamName = team.Name,
                                    EmployeeName = user.Name,
                                    Category = task.Category.Categories,
                                    SubCategory = task.Category.SubCategory,
                                    Status = task.Status,
                                    Percentage = task.Percentage,
                                    EstimateTime = task.EstTime,
                                    ActualTime = task.ActTime,
                                    TeamId = task.TeamId,
                                    EstimateStartDate = task.EstimateStartDate,
                                    EstimateEndDate = task.EstimateEndDate,
                                    ActualStartDate = task.ActualStartDate,
                                    ActualEndDate = task.ActualEndDate,
                                    weekEndDate = task.WeekEndingDate
                                }).Distinct().OrderByDescending(x => x.Id).ToList();

                return taskList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<Task> CreateTask(User user, TaskDTO task)
        {
            try
            {
                UserStoryUI UserStoryUi = new();
                var UserStoryUiId = await _repository.UserStoryUI.FindByConditionAsync(x => x.UserStoryId == task.UserStoryId && x.UIId == task.UIId);
                if (UserStoryUiId.Any())
                {
                    UserStoryUi = UserStoryUiId.First();
                }
                DateTime currentDate = DateTime.Today;

                var teamId = await _dbContext.TeamProject.Where(x => x.ProjectId == task.ProjectId).FirstAsync();

                var EmployeeProjectTask = _mapper.Map<Task>(task);
                if (UserStoryUi != null)
                {
                    EmployeeProjectTask.UserStoryUIId = UserStoryUi.Id;
                }

                EmployeeProjectTask.CreatedBy = user.Id.ToString();
                EmployeeProjectTask.UpdatedBy = user.Id.ToString();
                EmployeeProjectTask.CreatedDate = DateTime.Now;
                EmployeeProjectTask.UpdatedDate = DateTime.Now;
                EmployeeProjectTask.TeamId = teamId.TeamId;
                 var taskcreated = await _repository.Task.CreateAsync(EmployeeProjectTask);
                Comments comment = new()
                {
                    ProjectId = task.ProjectId,
                    TaskId = taskcreated.Id,
                    EmployeeId = task.EmployeeId,
                    CreatedBy = user.Id.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedBy = user.Id.ToString(),
                    Comment = task.Comment,
                };
                var addComments = await _repository.Comments.CreateAsync(comment);
                return taskcreated;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<UserInterface>> GetUserInterfacelist(int UserStoryId)
        {
            try
            {
                var USUIList = await _repository.UserStoryUI.GetUIlist(UserStoryId);
                var UIList = await _repository.UserInterface.FindByConditionAsync(app => USUIList.Contains(app.Id));
                var result = UIList.ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<UserStory>> GetProjectUSlist(int ProjectId)
        {
            try
            {
                var USList = await _repository.UserStory.FindByConditionAsync(x => x.ProjectId == ProjectId);
                var result = USList.ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TaskListDto>> GetEmployeeTaskList(int employeeId)
        {
            try
            {
                var taskList = (from task in _dbContext.Task
                                join project in _dbContext.Project on task.ProjectId equals project.Id into projectJoin
                                from project in projectJoin.DefaultIfEmpty()
                                join comment in _dbContext.Comments on task.Id equals comment.TaskId into commentJoin
                                from comment in commentJoin.DefaultIfEmpty()
                                join employeeTask in _dbContext.EmployeeTask on task.Id equals employeeTask.TaskId into employeeTaskJoin
                                from employeeTask in employeeTaskJoin.DefaultIfEmpty()
                                join team in _dbContext.Team on task.TeamId equals team.Id into teamJoin
                                from team in teamJoin.DefaultIfEmpty()
                                join employee in _dbContext.Employee on employeeTask.EmployeeId equals employee.Id into employeeJoin
                                from employee in employeeJoin.DefaultIfEmpty()
                                join user in _dbContext.Users on employee.UserId equals user.Id into userJoin
                                from user in userJoin.DefaultIfEmpty()
                                join userStoryUI in _dbContext.UserStoryUI on task.UserStoryUIId equals userStoryUI.Id into userStoryUIJoin
                                from userStoryUI in userStoryUIJoin.DefaultIfEmpty()
                                join userStory in _dbContext.UserStory on userStoryUI.UserStoryId equals userStory.Id into userStoryJoin
                                from userStory in userStoryJoin.DefaultIfEmpty()
                                join userInterface in _dbContext.UserInterface on userStoryUI.UIId equals userInterface.Id into userInterfaceJoin
                                from userInterface in userInterfaceJoin.DefaultIfEmpty()
                                where employee.Id == employeeId
                                select new TaskListDto()
                                {
                                    Id = task.Id,
                                    Name = task.Name,
                                    projectName = project.Name,
                                    Description = task.Description,
                                    TeamName = team.Name,
                                    EmployeeName = user.Name,
                                    Category = task.Category.Categories,
                                    SubCategory = task.Category.SubCategory,
                                    Status = task.Status,
                                    Comment = comment.Comment,
                                    Percentage = task.Percentage,
                                    EstimateTime = task.EstTime,
                                    ActualTime = task.ActTime,
                                    EstimateStartDate = task.EstimateStartDate,
                                    EstimateEndDate = task.EstimateEndDate,
                                    ActualStartDate = task.ActualStartDate,
                                    ActualEndDate = task.ActualEndDate,
                                    weekEndDate = task.WeekEndingDate
                                }).ToList();

                return taskList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Task>> GetTaskListValue()
        {
            try
            {
                var taskList = _dbContext.Task.Include(o => o.Project).Distinct().OrderByDescending(x => x.Id).ToList();               
                return taskList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<CommentsDto>> GetComments(int taskId)
        {
            try
            {
                var commentList = (from comment in _dbContext.Comments
                                   join employee in _dbContext.Employee on comment.EmployeeId equals employee.Id
                                   join user in _dbContext.Users on employee.UserId equals user.Id
                                   where comment.TaskId == taskId
                                   select new CommentsDto()
                                   {
                                       Id = comment.Id,
                                       Comment = comment.Comment,
                                       Employee = user.Name,
                                       EmployeeTimeId = comment.EmployeeTimeId,
                                       CreatedOn = comment.CreatedDate,
                                       TaskId = comment.TaskId,
                                       ProjectId = comment.ProjectId
                                   }).OrderByDescending(x => x.Id).ToList();
                return commentList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Task> CreateTaskCheckList(User user, TaskCheckListDto CheckList)
        {
            try
            {
                UserStoryUI UserStoryUi = new();
                var UserStoryUiId = await _repository.UserStoryUI.FindByConditionAsync(x => x.UserStoryId == CheckList.UserStoryId && x.UIId == CheckList.UIId);
                if (UserStoryUiId.Any())
                {
                    UserStoryUi = UserStoryUiId.First();
                }
                DateTime currentDate = DateTime.Today;

                var teamId = await _dbContext.TeamProject.Where(x => x.ProjectId == CheckList.ProjectId).FirstAsync();

                var EmployeeProjectTask = _mapper.Map<Task>(CheckList);
                if (UserStoryUi != null)
                {
                    EmployeeProjectTask.UserStoryUIId = UserStoryUi.Id;
                }

                EmployeeProjectTask.CreatedBy = user.Id.ToString();
                EmployeeProjectTask.UpdatedBy = user.Id.ToString();
                EmployeeProjectTask.CreatedDate = DateTime.Now;
                EmployeeProjectTask.UpdatedDate = DateTime.Now;
                EmployeeProjectTask.TeamId = teamId.TeamId;
                var taskcreated = await _repository.Task.CreateAsync(EmployeeProjectTask);

                Comments comment = new()
                {
                    ProjectId = CheckList.ProjectId,
                    TaskId = taskcreated.Id,
                    EmployeeId = CheckList.EmployeeId,
                    CreatedBy = user.Id.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedBy = user.Id.ToString(),
                    Comment = CheckList.Comment,
                };
                var addComments = await _repository.Comments.CreateAsync(comment);

                foreach (var description in CheckList.CheckListDescriptions)
                {
                    TaskCheckList taskCheckListDescription = new()
                    {
                        TaskId = taskcreated.Id,
                        ProjectId = CheckList.ProjectId,
                        USId = CheckList.UserStoryId,
                        CategoryId = CheckList.CategoryId,
                        UIId = CheckList.UIId,
                        CheckListDescription = description,
                        CreatedBy = user.Id.ToString(),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = user.Id.ToString(),
                    };
                var addCheckList = await _repository.TaskCheckList.CreateAsync(taskCheckListDescription);
                }
                return taskcreated;

                
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
