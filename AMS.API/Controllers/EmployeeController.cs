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

namespace ProjectOversight.API.Controllers
{
    [Route("v1/app/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IUnitOfWork repository, UserManager<User> userManager,
                                 IConfiguration configuration, IMapper mapper, SignInManager<User> signInManager,
                                IEmployeeService employeeService)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _signInManager = signInManager;
            _employeeService = employeeService;
        }







        [HttpGet("GetEmployeeList")]
        public async Task<ActionResult<IEnumerable<EmployeeDailyTask>>> GetEmployeeList()
        {
            try
            {
                var result = await _employeeService.GetEmployeeList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        [HttpPost("UpdateEmployee")]

        public async Task<ActionResult> UpdateEmployee(UserCreateDto userCreateDto)
        {
            try
            {
                var result = await _employeeService.UpdateEmployee(userCreateDto);
                return Ok("Employee update successfully!");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("GetEmployeeStatDetails")]
        public async Task<ActionResult<EmployeeStatDto>> GetEmployeeStatDetails()
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _employeeService.GetEmployeeStatDetails(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("AssignSkill")]
        public async Task<ActionResult> AssignSkill(EmployeeSkillSet[] empSkillSet)
        {
            try
            {
                var result = await _employeeService.AssignSkill(empSkillSet);
                return Ok("Employee Skill Updated!");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetEmployeeSkillById")]
        public async Task<ActionResult<IEnumerable<EmployeeSkillSet>>> GetEmployeeSkillById(int employeeId)
        {
            try
            {
                var result = await _employeeService.GetEmployeeSkillById(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetEmployeeTask")]
        public async Task<ActionResult<List<TeamDto>>> GetEmployeeTask(int teamId, DateTime? weekend)
        {
            try
            {
                var result = await _employeeService.GetEmployeeTask(teamId, weekend);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetEmployeeProject")]
        public async Task<ActionResult<List<EmployeeProject>>> GetEmployeeProject(int projectId)
        {
            try
            {
                var result = await _employeeService.GetEmployeeProject(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetEmployeeAttendance")]
        public async Task<ActionResult<AttendanceDto>> GetEmployeeAttendance(DateTime selectedDate)
        {
            try
            {
                var result = await _employeeService.GetEmployeeAttendance(selectedDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetAttendanceByEmployeeId")]
        public async Task<ActionResult<EmployeeAttendanceVM>> GetAttendanceByEmployeeId(int userId)
        {
            try
            {
                var result = await _employeeService.GetAttendanceByEmployeeId(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetEmployeeTasks")]
        public async Task<ActionResult<EmployeeTaskVM>> GetEmployeeTasks(int employeeId)
        {
            try
            {
                var result = await _employeeService.GetEmployeeTasks(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPut("UpdateByEmployeeId")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> UpdateByEmployeeId(int UserId, [FromBody] EmployeeDto employeeDto )
        //{
        //    try
        //    {
        //        var result = await _employeeService.UpdateByEmployeeId(UserId,employeeDto);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        [HttpPut("UpdateByEmployeeId")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateByEmployeeId(int userId, [FromForm] EmployeeDto employeeDto)
        {
            try
            {
                if (employeeDto.ProfilePhotoBase64 != null)
                {
                    byte[] fileBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        await employeeDto.ProfilePhotoBase64.CopyToAsync(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }
                    string base64String = Convert.ToBase64String(fileBytes);
                   

                    var employeeDetails = new Employee
                    {
                        UserId = userId,
                        Name = employeeDto.Name,
                        ProfilePhoto = base64String,
                        PhoneNumber = employeeDto.PhoneNumber,

                    };

                    return Ok(employeeDetails);
                }
                else
                {
                    return BadRequest("Profile photo is required.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the employee.");
            }
        }


    }

}
