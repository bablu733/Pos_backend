using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services.Interface;

namespace ProjectOversight.API.Controllers
{
    [Route("v1/app/[controller]")]
    [ApiController]
    //[Authorize]
    public class CommonMasterController : ControllerBase
    {
        private readonly ICommonMasterService _commonMasterService;
        private readonly ILogErrorService _logError;
        private readonly UserManager<User> _userManager;

        public CommonMasterController(ICommonMasterService commonMasterService, ILogErrorService logError, UserManager<User> userManager)
        {
            _commonMasterService = commonMasterService;
            _logError = logError;
            _userManager = userManager;
        }

        [HttpGet("GetCodeTableList")]
        public async Task<IActionResult> GetCodeTableList()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            try
            {
                var codeTable = await _commonMasterService.GetCodeTableList();
                return Ok(codeTable);
            }
            catch (Exception ex)
            {
                LogErrorDto logErrorDto = new LogErrorDto();
                logErrorDto.ControllerName = "CommonMaster";
                logErrorDto.MethodName = "GetCodeTableList";
                logErrorDto.TableName = "CommonMaster";
                logErrorDto.LogDescription = ex.Message + " , " + ex.InnerException != null ? ex.Message : ex.InnerException.Message;

                var result = await _logError.AddErrorLogs(logErrorDto, user);
                return Ok(result);
            }
        }
    }
}
