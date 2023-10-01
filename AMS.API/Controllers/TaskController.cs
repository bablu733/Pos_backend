using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services.Interface;
using Task = ProjectOversight.API.Data.Model.Task;

namespace ProjectOversight.API.Controllers
{
    [Route("v1/app/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly ITaskService _taskService;

        public TaskController(IUnitOfWork repository, UserManager<User> userManager,
        IConfiguration configuration, IMapper mapper, SignInManager<User> signInManager, ITaskService taskService)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _signInManager = signInManager;
            _taskService = taskService;
        }

        [HttpGet("GetTaskList")]
        public async Task<ActionResult<IEnumerable<TaskListDto>>> GetTaskList()
        {
            try
            {
                var result = await _taskService.GetTaskList();
                if (result.Count() > 0)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetTaskListValue")]
        public async Task<ActionResult<IEnumerable<TaskListDto>>> GetTaskListValue()
        {
            try
            {
                var result = await _taskService.GetTaskListValue();
                if (result.Count() > 0)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost("CreateTask")]
        public async Task<ActionResult<Task>> CreateTask([FromBody] TaskDTO task)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _taskService.CreateTask(user, task);
                if (result != null)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetUserInterfacelist")]
        public async Task<ActionResult<IEnumerable<UserStory>>> GetUserInterfacelist(int UserStoryId)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _taskService.GetUserInterfacelist(UserStoryId);
                if (result.Count() > 0)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetProjectUSlist")]
        public async Task<ActionResult<IEnumerable<UserStory>>> GetProjectUSlist(int ProjectId)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _taskService.GetProjectUSlist(ProjectId);
                if (result.Count() > 0)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetEmployeeTaskList")]
        public async Task<ActionResult<List<TaskListDto>>> GetEmployeeTaskList(int employeeId)
        {
            try
            {
                var result = await _taskService.GetEmployeeTaskList(employeeId);
                if (result.Count() > 0)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetComments")]
        public async Task<ActionResult<List<CommentsDto>>> GetComments(int taskId)
        {
            try
            {
                var result = await _taskService.GetComments(taskId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("CreateTaskCheckList")]
        public async Task<ActionResult<Task>> CreateTaskCheckList([FromBody] TaskCheckListDto CheckList)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _taskService.CreateTaskCheckList(user, CheckList);
                if (result != null)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
