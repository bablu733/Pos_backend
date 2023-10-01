using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectOversight.API.Data;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services;
using ProjectOversight.API.Services.Interface;

namespace ProjectOversight.API.Controllers
{
    [Route("v1/app/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeTimeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmployeeTimeService _employeeTimeService;
        private readonly ILogErrorService _logErrorService;

        public EmployeeTimeController(IUnitOfWork repository, UserManager<User> userManager,
        IConfiguration configuration, IMapper mapper, SignInManager<User> signInManager, IEmployeeTimeService employeeTimeService, ILogErrorService logErrorService)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _signInManager = signInManager;
            _employeeTimeService = employeeTimeService;
            _logErrorService = logErrorService;
        }

        [HttpPost("AddEmployeeTimeDetails")]
        public async Task<ActionResult<IEnumerable<bool>>> AddEmployeeTimeDetails([FromBody] EmployeeTimeDto LoginDetails)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);

            try
            {
              
                var result = await _employeeTimeService.AddEmployeeTimeDetails(user, LoginDetails);
                if (result == true)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "EmployeeTimeController";
                logErrorDto.MethodName = "AddEmployeeTimeDetails";
                logErrorDto.TableName = "EmployeeTimeDto";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }
        }

        [HttpGet("GetEmployeeTimeDetails")]
        public async Task<ActionResult<IEnumerable<EmployeeTime>>> GetEmployeeTimeDetails()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            try
            {
                
                var result = await _employeeTimeService.GetEmployeeTimeDetails(user);

                return Ok(result);
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "EmployeeTimeController";
                logErrorDto.MethodName = "GetEmployeeTimeDetails";
                logErrorDto.TableName = "EmployeeTime";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }
        }

        [HttpGet("GetComments")]
        public async Task<ActionResult<List<CommentsDto>>> GetComments()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            try
            {
                var result = await _employeeTimeService.GetComments();
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "EmployeeTimeController";
                logErrorDto.MethodName = "GetComments";
                logErrorDto.TableName = "CommentsDto";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }
        }


    }
}
