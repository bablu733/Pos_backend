using AutoMapper;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Data;
using ProjectOversight.API.Services.Interface;
using ProjectOversight.API.Data.Model;
using Task = ProjectOversight.API.Data.Model.Task;
using Comment = ProjectOversight.API.Data.Model.Comments;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Constants;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProjectOversight.API.Services
{
    public class CommonService : ICommonService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;
        private readonly ProjectOversightContext _dbContext;
        public CommonService(IUnitOfWork repository, IMapper mapper, ProjectOversightContext dbContext)
        {
            _mapper = mapper;
            _repository = repository;
            _dbContext = dbContext;
        }
        public async Task<List<Category>> GetCategoriesList()
        {
            try
            {
                var categoryList = await _repository.Category.FindAllAsync();
                var result = categoryList.ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<Task>> GetProjectTaskList(int ProjectId)
        {
            try
            {
                var ProjectList = await _repository.Task.FindByConditionAsync(x => x.ProjectId == ProjectId);
                var result = ProjectList.ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> AddComment(User user,Comment comment)
        {
            try
            {
                Comment newComment = new Comment()
                {
                    Comment = comment.Comment,
                    TaskId = comment.TaskId,
                    ProjectId = comment.ProjectId,
                    EmployeeId = comment.EmployeeId,
                    CreatedBy = user.Id.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedBy = user.Id.ToString(),
                    UpdatedDate = DateTime.Now,
                };
                _dbContext.Comments.Add(newComment);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<CommentsDto>> GetCommentsList(int TaskId)
        {
            try
            {
                var comments = await _repository.Comments.FindByConditionAsync(x => x.TaskId == TaskId);
                var employeeId = comments.Select(x => x.EmployeeId).ToList();
                var employees = await _repository.Employee.FindByConditionAsync(x => employeeId.Contains(x.Id));
                //var user = await _dbContext.Users.Where(x=>x.Id == employees.FirstOrDefault().UserId).FirstOrDefault();
                var userId = employees.Select(x => x.UserId).ToList();
                var users = await _repository.User.FindByConditionAsync(x => userId.Contains(x.Id));
                var task = await _repository.Task.FindByConditionAsync(x => x.Id == TaskId);

                var employeeTask = comments.Select(x => x.EmployeeTaskId).ToList();
                var employeesTaskId = await _repository.EmployeeTask.FindByConditionAsync(x => employeeTask.Contains(x.Id));

                var employeeDailyTask = comments.Select(x => x.EmployeeDailyTaskId).ToList();
                var employeesDailyTaskId = await _repository.DailyTask.FindByConditionAsync(x => employeeDailyTask.Contains(x.Id));
                var commentWithEmployeeNames = comments
         .Join(employees,
             comment => comment.EmployeeId,
             employee => employee.Id,
             (comment, employee) => new { comment, employee })
         .Join(users,
             joinResult => joinResult.employee.UserId,
             user => user.Id,
             (joinResult, user) => new CommentsDto
             {
                 ProjectId = joinResult.comment.ProjectId,
                 TaskId = joinResult.comment.TaskId,
                 EmployeeTaskId = joinResult.comment.EmployeeTaskId,
                 EmployeeDailyTaskId = joinResult.comment.EmployeeDailyTaskId,
                 EmployeeId = joinResult.comment.EmployeeId,
                 EmployeeTimeId = joinResult.comment.EmployeeTimeId,
                 Comment = joinResult.comment.Comment,
                 CreatedDate = joinResult.comment.CreatedDate,
                 EmployeeName = user.Name,
             }).ToList();

                foreach (var comment in commentWithEmployeeNames)
                {
                    if (comment.TaskId!=null && comment.EmployeeDailyTaskId != null && comment.EmployeeTaskId != null)
                    {
                        comment.Status = employeesDailyTaskId.Where(x => x.Id == comment.EmployeeDailyTaskId).FirstOrDefault().Status;
                        comment.Percentage = employeesDailyTaskId.Where(x => x.Id == comment.EmployeeDailyTaskId).FirstOrDefault().Percentage;
                    }
                    else if (comment.TaskId != null && comment.EmployeeTaskId != null && comment.EmployeeDailyTaskId == null)
                    {
                        comment.Status = employeesTaskId.Where(x => x.Id == comment.EmployeeTaskId).FirstOrDefault().Status;
                        comment.Percentage = employeesTaskId.Where(x => x.Id == comment.EmployeeTaskId).FirstOrDefault().Percentage;
                    }
                    else if (comment.TaskId != null && comment.EmployeeTaskId == null && comment.EmployeeDailyTaskId == null)
                    {
                        comment.Status = task.Where(x => x.Id == TaskId).FirstOrDefault().Status;
                        comment.Percentage = task.Where(x => x.Id == TaskId).FirstOrDefault().Percentage;
                    }
                }
                return commentWithEmployeeNames;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Comments> AddReplyComments(User user, CommentsDto ComDetails)
        {
            try
            {
                if (ComDetails.EmployeeTaskId == null && ComDetails.TaskId != null && ComDetails.EmployeeDailyTaskId == null)
                {
                    Comments comment = new()
                    {
                        ProjectId = ComDetails.ProjectId,
                        TaskId = ComDetails.TaskId,
                        EmployeeId = ComDetails.EmployeeId,
                        CreatedBy = user.Id.ToString(),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = user.Id.ToString(),
                        Comment = ComDetails.Comment,
                    };
                    var addComments = await _repository.Comments.CreateAsync(comment);
                    return addComments;
                }
                else if (ComDetails.EmployeeDailyTaskId != null && ComDetails.EmployeeTaskId != null)
                {
                    Comments comment = new()
                    {
                        ProjectId = ComDetails.ProjectId,
                        TaskId = ComDetails.TaskId,
                        EmployeeId = ComDetails.EmployeeId,
                        EmployeeTaskId = ComDetails.EmployeeTaskId,
                        EmployeeDailyTaskId = ComDetails.EmployeeDailyTaskId,
                        CreatedBy = user.Id.ToString(),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = user.Id.ToString(),
                        Comment = ComDetails.Comment,
                    };
                    var addComments = await _repository.Comments.CreateAsync(comment);
                    return addComments;
                }
                else
                {
                    Comments comment = new()
                    {
                        ProjectId = ComDetails.ProjectId,
                        TaskId = ComDetails.TaskId,
                        EmployeeId = ComDetails.EmployeeId,
                        EmployeeTaskId = ComDetails.EmployeeTaskId,
                        CreatedBy = user.Id.ToString(),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = user.Id.ToString(),
                        Comment = ComDetails.Comment,
                    };
                    var addComments = await _repository.Comments.CreateAsync(comment);
                    return addComments;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
