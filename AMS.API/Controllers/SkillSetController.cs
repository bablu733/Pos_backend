
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
    public class SkillsetController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly ISkillsetService _skillsetService;
        private readonly ILogErrorService _logErrorService;

        public SkillsetController(IUnitOfWork repository, UserManager<User> userManager,
        IConfiguration configuration, IMapper mapper, SignInManager<User> signInManager, ISkillsetService skillsetService, ILogErrorService logErrorService)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _signInManager = signInManager;
            _skillsetService = skillsetService;
            _logErrorService = logErrorService;
        }

        [HttpGet("GetSkillsetList")]
        public async Task<ActionResult<IEnumerable<SkillSet>>> GetSkillsetList()
        {
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);

                try
                {
                    var result = await _skillsetService.GetSkillsetList();
                    if (result.Count() > 0)
                        return Ok(result);
                    else
                        return NoContent();
                }
                catch (Exception ex)
                {
                    LogErrorDto logErrorDto = new LogErrorDto();
                    logErrorDto.ControllerName = "SkillsetController";
                    logErrorDto.MethodName = "GetSkillsetList";
                    logErrorDto.TableName = "SkillSet";
                    logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                    var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                    return Ok(result);
                }
            }
        }
        [HttpGet("GetSkillsetById")]
        [AllowAnonymous]
            
        public async Task<ActionResult<IEnumerable<SkillSet>>> GetSkillsetById(int Id)
           {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            try
               {
                  var result = await _skillsetService.GetSkillsetById(Id);
                  return Ok(result);
               }
               catch (Exception ex)
              {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "SkillsetController";
                logErrorDto.MethodName = "GetSkillsetById";
                logErrorDto.TableName = "SkillSet";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }

        }
        [HttpPost("AddSkillset")]
        public async Task<ActionResult<IEnumerable<bool>>> AddSkillset([FromBody] SkillsetDto Skillset)

        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            try
            { 
                var result = await _skillsetService.AddSkillset(user, Skillset);
                if (result == true)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "SkillsetController";
                logErrorDto.MethodName = "AddSkillset";
                logErrorDto.TableName = "SkillsetDto";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }


        }
       

        [HttpPost("UpdateSkillset")]
        public async Task<ActionResult<bool>> UpdateSkillset([FromBody] SkillsetDto updatedSkillset)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            try
            {
                
                var result = await _skillsetService.UpdateSkillset(user, updatedSkillset);

                if (result)
                    return Ok(result);
                else
                    return NotFound(); 
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "SkillsetController";
                logErrorDto.MethodName = "UpdateSkillset";
                logErrorDto.TableName = "SkillsetDto";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logErrorService.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }
        }
    }
}
