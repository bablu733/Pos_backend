using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services;
using ProjectOversight.API.Services.Interface;
using Task = ProjectOversight.API.Data.Model.Task;

namespace ProjectOversight.API.Controllers
{
    [Route("v1/app/[controller]")]
    [ApiController]
    [Authorize]
    public class CommonController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly ICommonService _commonService;
        private readonly ILogErrorService _logErrorService;
        public CommonController(IUnitOfWork repository, UserManager<User> userManager,
        IConfiguration configuration, IMapper mapper, SignInManager<User> signInManager, ICommonService commonService, ILogErrorService logErrorService)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _signInManager = signInManager;
            _commonService = commonService;
            _logErrorService = logErrorService;
        }

        [HttpGet("GetCategoriesList")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesList()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            try
            {
                var result = await _commonService.GetCategoriesList();
                if (result.Count() > 0)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "CommonnConntroller";
                logErrorDto.MethodName = "GetCategoriesList";
                logErrorDto.TableName = "Category";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }
        }
        [HttpGet("GetProjectTaskList")]
        public async Task<ActionResult<IEnumerable<Task>>> GetProjectTaskList(int ProjectId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            try
            {
                var result = await _commonService.GetProjectTaskList(ProjectId);
                if (result.Count() > 0)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "CommonnConntroller";
                logErrorDto.MethodName = "GetProjectTaskList";
                logErrorDto.TableName = "Task";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }
        }
        [HttpGet("GetCommentsList")]
        public async Task<ActionResult<IEnumerable<CommentsDto>>> GetCommentsList(int EmployeeTaskId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);

            try
            {
                var result = await _commonService.GetCommentsList(EmployeeTaskId);
                if (result.Count() > 0)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "CommonnConntroller";
                logErrorDto.MethodName = "GetCommentsList";
                logErrorDto.TableName = "CommentsDto";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }
        }

        [HttpPost("AddComment")]
        public async Task<ActionResult<bool>> AddComment(Comments comment)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            try
            {
                var result = await _commonService.AddComment(user, comment);
                return Ok("Comment Created");
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "CommonnConntroller";
                logErrorDto.MethodName = "AddComment";
                logErrorDto.TableName = "Comments";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }
        }


        [HttpPost("AddReplyComments")]
        [AllowAnonymous]
        public async Task<ActionResult<Comments>> AddReplyComments(CommentsDto ComDetails)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            try
            {
                var result = await _commonService.AddReplyComments(user, ComDetails);
                if (result != null)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "CommonnConntroller";
                logErrorDto.MethodName = "AddReplyComments";
                logErrorDto.TableName = "Comments";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }
        }
    }
}
