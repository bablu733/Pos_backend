using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services.Interface;
using System.Data.SqlTypes;
using Task = ProjectOversight.API.Data.Model.Task;

namespace ProjectOversight.API.Controllers
{
    [Route("v1/app/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IProjectService _projectService;
        public ProjectController(IUnitOfWork repository, UserManager<User> userManager,
        IConfiguration configuration, IMapper mapper, SignInManager<User> signInManager, IProjectService projectService)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _signInManager = signInManager;
            _projectService = projectService;
        }
        [HttpGet("GetEmployeeProjectlist")]
        public async Task<ActionResult<IEnumerable<Project>>> GetEmployeeProjectlist()

        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.GetEmployeeProjectlist(user);
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


        [HttpPost("CreateEmployeeDayPlan")]
        [AllowAnonymous]
        public async Task<ActionResult<EmployeeTask>> CreateEmployeeDayPlan(EmployeeTaskDto dayPlan)

        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.CreateEmployeeDayPlan(user, dayPlan);
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
        [HttpGet("GetAllProjectlist")]
        public async Task<ActionResult<IEnumerable<Project>>> GetAllProjectlist()

        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.GetAllProjectlist();
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
        [HttpGet("GetUserInterfacelist")]
        public async Task<ActionResult<IEnumerable<UserStory>>> GetUserInterfacelist(int projectId)
        {
            try
            {

                var result = await _projectService.GetUserInterfacelist(projectId);
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

        [HttpPost("AddUserInterface")]
        public async Task<ActionResult<bool>> AddUserInterface(UserInterface userInterface)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.AddUserInterface(user, userInterface);
                if (result)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("UpdateUserInterface")]
        public async Task<ActionResult<bool>> UpdateUserInterface(UserInterface userInterface)
        {
            try
            {
                var result = await _projectService.UpdateUserInterface(userInterface);
                if (result)
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
                var result = await _projectService.GetProjectUSlist(ProjectId);
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

        [HttpGet("GetCategoriesList")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesList()

        {
            try
            {
                //var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.GetCategoriesList();
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

        [HttpPost("AddEmployeeProject")]
        public async Task<ActionResult<IEnumerable<bool>>> AddEmployeeProject([FromBody] EmployeeProjectDto employeeProject)

        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.AddEmployeeProject(user, employeeProject);
                if (result == true)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }


        }


        [HttpGet("GetProjectList")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectList()
        {
            try
            {
                var result = await _projectService.GetProjectlist();
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

        [HttpGet("GetProjectById")]
        public async Task<ActionResult<Project>> GetProjectById(int Id)
        {
            try
            {
                var result = await _projectService.GetProjectById(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        [HttpPost("AddProject")]
        public async Task<IActionResult> AddProject(Project projectData)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.AddProject(user, projectData);
                if (result == true)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ModelState);
            }


        }
        [HttpPost("UpdateProject")]
        public async Task<IActionResult> UpdateProject(Project updatedProjectData)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.UpdateProject(user, updatedProjectData, updatedProjectData.Id);

                if (result)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetProjectObjective")]
        public async Task<ActionResult<IEnumerable<ProjectObjective>>> GetProjectObjective(int ProjectId)

        {
            try
            {
                //var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.GetProjectObjective(ProjectId);
                if (result.Count() > 0)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving project objectives.");
            }
        }

        [HttpGet("GetProjectObjectiveById")]
        public async Task<ActionResult<IEnumerable<ProjectObjective>>> GetProjectObjectiveById(int Id)

        {
            try
            {
                var result = await _projectService.GetProjectObjectiveById(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("AddProjectObjective")]
        public async Task<IActionResult> AddProjectObjective(ProjectObjective projectObjective)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.AddProjectObjective(user, projectObjective);
                if (result == true)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("UpdateProjectObjective")]
        public async Task<IActionResult> UpdateProjectObjective(ProjectObjective updatedProjectObjective)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.UpdateProjectObjective(user, updatedProjectObjective, updatedProjectObjective.Id);
                //var result = await _projectService.UpdateProjectObjective.FirstOrDefaultAsync(x => x.Id == updatedProjectObjective.Id);
                if (result)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("GetUserStoryList")]
        public async Task<ActionResult<IEnumerable<UserStory>>> GetUserStoryList(int projectId)
        {
            try
            {

                var result = await _projectService.GetUserStoryList(projectId);
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
        [HttpPost("AddUserStory")]

        public async Task<ActionResult> AddUserStory(UserStory UserStory)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.AddUserStory(user, UserStory);
                if (result == true)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost("UpdateUserStory")]
        public async Task<ActionResult> UpdateUserStory(UserStory updatedUserStory)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.UpdateUserStory(user, updatedUserStory);
                if (result == true)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("AddUserStoryUI")]
        public async Task<ActionResult<bool>> AddUserStoryUI(UserStoryUI[] userStoryUI)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.AddUserStoryUI(user, userStoryUI);
                if (result == true)
                    return Ok(result);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetUserStoryUIList")]
        public async Task<ActionResult<List<UserStoryUI>>> GetUserStoryUIList(int userStoryId)
        {
            try
            {
                var result = await _projectService.GetUserStoryUIList(userStoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("AssignEmployeeProject")]
        public async Task<ActionResult<bool>> AssignEmployeeProject(EmployeeProject[] employeeProject)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.AssignEmployeeProject(user, employeeProject);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("AssignLead")]
        public async Task<ActionResult<bool>> AssignLead(int userId, int projectId)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.AssignLead(user, userId, projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetUnAssignedProjects")]
        public async Task<ActionResult<List<Project>>> GetUnAssignedProjects(int TeamID)
        {
            try
            {
                var result = await _projectService.GetUnAssignedProjects(TeamID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetProjectStatDetails")]
        public async Task<ActionResult<ProjectStatDto>> GetProjectStatDetails(int ProjectId)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.GetProjectStatDetails(ProjectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("Updatetask")]
        public async Task<ActionResult<bool>> Updatetask(EmployeeTaskDto dayPlan)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.Updatetask(user, dayPlan);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("ResourcesAssignedToAnyProject")]
        public async Task<ActionResult<IEnumerable<Project>>> ResourcesAssignedToAnyProject()
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                var result = await _projectService.ResourcesAssignedToAnyProject();

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

        [HttpGet("ResourcesAssignedByProject")]
        public async Task<ActionResult<ProjectDto>> ResourcesAssignedByProject(int ProjectId)
        {
            try
            {
                var result = await _projectService.ResourcesAssignedByProject(ProjectId);

                if (result == null)
                {
                    return NotFound("Project not found.");
                }

                if (result.TotalTaskCount == 0 && result.UseInterfaceCount == 0 && result.UserStoryCount == 0)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

}
