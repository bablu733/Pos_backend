using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectOversight.API.Constants;
using ProjectOversight.API.Data;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services.Interface;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Threading.Tasks;
using Task = ProjectOversight.API.Data.Model.Task;

namespace ProjectOversight.API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ProjectOversightContext _dbContext;

        public ProjectService(IUnitOfWork repository, IMapper mapper, UserManager<User> userManager,
            RoleManager<Role> roleManager, IConfiguration configuration, ProjectOversightContext dbContext)
        {
            _mapper = mapper;
            _userManager = userManager;
            _repository = repository;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<List<Project>> GetAllProjectlist()
        {
            try
            {
                var projectList = await _repository.Project.FindAllAsync();
                var result = projectList.ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetEmployeeProjectlist(User user)
        {
            try
            {
                var employee = await _repository.Employee.FindById(x => x.UserId == user.Id);
                var employeeProject = await _repository.EmployeeProject.GetEmployeeProjectList(employee.Id);
                var projectList = await _repository.Project.FindByConditionAsync(x => employeeProject.Contains(x.Id));
                var result = projectList.ToList();
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
        public async Task<bool> AddEmployeeProject(User user, EmployeeProjectDto employeeProject)
        {
            try
            {
                var employee = await _repository.Employee.FindById(x => x.UserId == user.Id);
                EmployeeProject empProject = new()
                {
                    ProjectId = employeeProject.ProjectId,
                    EmployeeId = employeeProject.EmployeeId,
                    CreatedBy = employee.UserId.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedBy = employee.UserId.ToString(),
                };
                var employeeLoginDetails = await _repository.EmployeeProject.CreateAsync(empProject);

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<List<UserInterface>> GetUserInterfacelist(int projectid)
        {
            try
            {
                var UIList = _dbContext.UserInterface.Where(x => x.ProjectId == projectid).OrderByDescending(x => x.Id).ToList();
                var result = UIList.ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> AddUserInterface(User user, UserInterface userInterface)
        {
            try
            {
                UserInterface userInterface1 = new UserInterface()
                {
                    ProjectId = userInterface.ProjectId,
                    ProjectObjectiveId = userInterface.ProjectObjectiveId,
                    Name = userInterface.Name,
                    Description = userInterface.Description,
                    Status = userInterface.Status,
                    Complexity = userInterface.Complexity,
                    Percentage = userInterface.Percentage,
                    StartDate = userInterface.StartDate,
                    EndDate = userInterface.EndDate,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.Id.ToString(),
                    UpdatedBy = user.Id.ToString(),
                    UICategory = userInterface.UICategory,
                };
                await _dbContext.UserInterface.AddAsync(userInterface1);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateUserInterface(UserInterface userInterface)
        {
            try
            {
                var userInterface1 = await _dbContext.UserInterface.FirstOrDefaultAsync(x => x.Id == userInterface.Id);

                if (userInterface1 != null)
                {
                    userInterface1.ProjectId = userInterface1.ProjectId;
                    userInterface1.ProjectObjectiveId = userInterface.ProjectObjectiveId;
                    userInterface1.Name = userInterface.Name;
                    userInterface1.Description = userInterface.Description;
                    userInterface1.UICategory = userInterface.UICategory;
                    userInterface1.Status = userInterface.Status;
                    userInterface1.Complexity = userInterface.Complexity;
                    userInterface1.Percentage = userInterface1.Percentage;
                    userInterface1.StartDate = userInterface.StartDate;
                    userInterface1.EndDate = userInterface.EndDate;
                    userInterface1.UpdatedDate = DateTime.Now;
                }
                else
                {
                    return false;
                };

                _dbContext.UserInterface.Update(userInterface1);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<EmployeeTask> CreateEmployeeDayPlan(User user, EmployeeTaskDto dayPlan)
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
                var EmployeeDayPlan = _mapper.Map<EmployeeTask>(dayPlan);
                //EmployeeDayPlan.Status = "Pending";
                EmployeeDayPlan.CreatedBy = user.Id.ToString();
                EmployeeDayPlan.UpdatedBy = user.Id.ToString();
                EmployeeDayPlan.EstStartDate = (DateTime)dayPlan.StartDate;
                EmployeeDayPlan.EstEndDate = (DateTime)dayPlan.EndDate;
                EmployeeDayPlan.CreatedDate = DateTime.Now;
                EmployeeDayPlan.WeekEndingDate = weekEndingDate;
                var taskcreated = await _repository.EmployeeTask.CreateAsync(EmployeeDayPlan);


                Comments comment = new()
                {
                    ProjectId = dayPlan.ProjectId,
                    TaskId = dayPlan.TaskId,
                    EmployeeId = dayPlan.EmployeeId,
                    EmployeeTaskId = taskcreated.Id,
                    CreatedBy = user.Id.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedBy = user.Id.ToString(),
                    Comment = dayPlan.Comment,
                };
                var addComments = await _repository.Comments.CreateAsync(comment);

                return taskcreated;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Updatetask(User user, EmployeeTaskDto dayPlan)
        {
            var task = _dbContext.Task.FirstOrDefault(x => x.Id == dayPlan.TaskId);
            var employeeTask = _dbContext.EmployeeTask.FirstOrDefault(x => x.TaskId == dayPlan.TaskId);
            if (employeeTask != null)
            {
                employeeTask.EmployeeId = dayPlan.EmployeeId;
                employeeTask.StartDate = (DateTime)dayPlan.StartDate;
                employeeTask.EndDate = (DateTime)dayPlan.EndDate;
                _dbContext.EmployeeTask.Update(employeeTask);
            }

            task.Priority = dayPlan.Priority;
            task.ProjectId = dayPlan.ProjectId;
            task.EstTime = dayPlan.EstTime;
            task.UpdatedDate = DateTime.Now;
            task.Status = dayPlan.Status;
            task.UpdatedBy = user.Id.ToString();
            _dbContext.Task.Update(task);

            var comment = _dbContext.Comments.FirstOrDefault(x => x.TaskId == dayPlan.TaskId);

            if (comment != null)
            {
                comment.Comment = dayPlan.Comment;
                comment.UpdatedBy = user.Id.ToString();
                comment.UpdatedDate = DateTime.Now;
                _dbContext.Comments.Update(comment);
            }

            _dbContext.SaveChanges();
            return true;
        }

        public async Task<List<ProjectDto>> GetProjectlist()
        {
            var projectList = (from project in _dbContext.Project
                               select new ProjectDto()
                               {
                                   Id = project.Id,
                                   Name = project.Name,
                                   Type = project.Type,
                                   Description = project.Description,
                                   Status = project.Status,
                                   Percentage = project.Percentage,
                                   StartDate = project.StartDate,
                                   EndDate = project.EndDate,
                                   ProjectDocuments = _dbContext.ProjectDocuments.FirstOrDefault(x => x.ProjectId == project.Id),
                                   ProjectTechStacks = _dbContext.ProjectTechStack.Where(x => x.ProjectId == project.Id).ToList(),
                                   UserStoryCount = _dbContext.UserStory.Where(x => x.ProjectId == project.Id).Count(),
                                   InProgressCount = _dbContext.Task.Where(x => x.Status == "Pending" && x.ProjectId == project.Id).Count(),
                                   TotalTaskCount = _dbContext.Task.Where(x => x.ProjectId == project.Id).Count(),
                                   NotStartedTaskCounts = _dbContext.Task.Where(x => x.ProjectId == project.Id && x.Status == "Unassigned").Count(),
                                   UseInterfaceCount = _dbContext.UserInterface.Where(x => x.ProjectId == project.Id).Count(),
                                   TeamId = _dbContext.TeamProject.Where(x => x.ProjectId == project.Id && x.EndDate == null).Select(x => x.TeamId).FirstOrDefault(),
                               }).OrderByDescending(x => x.Id).ToList();
            return projectList;
        }
        public async Task<Project> GetProjectById(int Id)
        {
            var project = await _dbContext.Project.Include(o => o.ProjectDocuments)
       .Include(p => p.ProjectTechStacks).ThenInclude(o => o.CommonMaster)
       .ToListAsync();
            var projectValue = project.Where(x => x.Id == Id).First();
            return projectValue;
        }

        public async Task<bool> AddProject(User user, Project project)
        {
            try
            {
                var employee = await _repository.Employee.FindById(x => x.UserId == user.Id);
                Project myProject = new()
                {
                    Name = project.Name,
                    Type = project.Type,
                    Description = project.Description,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Status = project.Status,
                    Percentage = project.Percentage,
                    CreatedBy = user.Id.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedBy = employee.UserId.ToString(),
                };
                var projectDetails = await _repository.Project.CreateAsync(myProject);
                foreach (var Id in project.TechStackId)
                {
                    ProjectTechStack myTechSrack = new()
                    {
                        ProjectId = projectDetails.Id,
                        TechStack = Id,
                        CreatedBy = user.Id.ToString(),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = employee.UserId.ToString(),
                    };
                    var employeeLoginDetails = await _repository.ProjectTechStack.CreateAsync(myTechSrack);
                }
                if (project.TeamId > 0)
                {
                    TeamProject teamProject = new()
                    {
                        ProjectId = projectDetails.Id,
                        TeamId = project.TeamId,
                        CreatedBy = user.Id.ToString(),
                        StartDate = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = employee.UserId.ToString(),
                    };
                    var addTeamProject = await _repository.TeamProject.CreateAsync(teamProject);

                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> UpdateProject(User user, Project project, int Id)
        {
            try
            {
                var employee = await _repository.Employee.FindById(x => x.UserId == user.Id);
                var projectValue = await _dbContext.Project.Include(o => o.ProjectDocuments)
                    .Include(p => p.ProjectTechStacks).ThenInclude(o => o.CommonMaster)
                    .FirstOrDefaultAsync(x => x.Id == Id);

                if (projectValue != null)
                {
                    projectValue.Name = project.Name;
                    projectValue.Type = project.Type;
                    projectValue.Description = project.Description;
                    projectValue.StartDate = project.StartDate;
                    projectValue.EndDate = project.EndDate;
                    projectValue.Status = project.Status;
                    projectValue.Percentage = project.Percentage;
                    projectValue.UpdatedDate = DateTime.Now;
                    projectValue.UpdatedBy = employee.UserId.ToString();
                    await _repository.Project.UpdateAsync(projectValue);
                }
                else
                {
                    return false;
                }
                if (project.TeamId > 0)
                {
                    var teamProject = _dbContext.TeamProject.Where(x => x.ProjectId == Id && x.EndDate == null).FirstOrDefault();
                    if (teamProject != null)
                    {
                        var teamProjectValue = teamProject.TeamId == project.TeamId;
                        if (teamProjectValue != true)
                        {
                            teamProject.EndDate = DateTime.Now;
                            teamProject.UpdatedDate = DateTime.Now;
                            teamProject.UpdatedBy = employee.UserId.ToString();
                            var updateTemproject = await _repository.TeamProject.UpdateAsync(teamProject);
                        }

                        if (teamProjectValue != true)
                        {
                            TeamProject teamProject1 = new()
                            {
                                ProjectId = projectValue.Id,
                                TeamId = project.TeamId,
                                CreatedBy = user.Id.ToString(),
                                StartDate = DateTime.Now,
                                CreatedDate = DateTime.Now,
                                UpdatedBy = employee.UserId.ToString(),
                            };
                            var addTeamProject = await _repository.TeamProject.CreateAsync(teamProject1);

                        }


                    }
                }

                var projectTech = await _repository.ProjectTechStack.FindAllAsync();
                var projectTechVal = projectTech.Where(x => x.ProjectId == projectValue.Id);
                var techIdsToDelete = projectTechVal.Where(x => !project.TechStackId.Contains(x.TechStack)).Select(x => x.Id).ToList();

                foreach (var techIdToDelete in techIdsToDelete)
                {
                    var projectTechToDelete = projectTech.FirstOrDefault(x => x.Id == techIdToDelete);

                    if (projectTechToDelete != null)
                    {
                        await _repository.ProjectTechStack.DeleteAsync(projectTechToDelete);
                    }
                }


                foreach (var techId in project.TechStackId)
                {
                    var existingProjectTech = projectTech.FirstOrDefault(x => x.TechStack == techId && x.ProjectId == projectValue.Id);

                    if (existingProjectTech != null)
                    {
                        existingProjectTech.UpdatedDate = DateTime.Now;
                        existingProjectTech.UpdatedBy = employee.UserId.ToString();
                        await _repository.ProjectTechStack.UpdateAsync(existingProjectTech);
                    }
                    else
                    {
                        ProjectTechStack newProjectTech = new()
                        {
                            ProjectId = projectValue.Id,
                            TechStack = techId,
                            CreatedBy = user.Id.ToString(),
                            CreatedDate = DateTime.Now,
                            UpdatedBy = employee.UserId.ToString(),
                        };
                        await _repository.ProjectTechStack.CreateAsync(newProjectTech);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<List<ProjectObjective>> GetProjectObjective(int ProjectId)
        {
            try
            {

                var projectObjective = await _dbContext.ProjectObjective.Where(x => x.ProjectId == ProjectId).ToListAsync();
                return projectObjective;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ProjectObjective> GetProjectObjectiveById(int Id)
        {
            var projectObjective = await _dbContext.ProjectObjective
        .SingleOrDefaultAsync(x => x.Id == Id);
            return projectObjective;
        }


        public async Task<bool> AddProjectObjective(User user, ProjectObjective projectObjective)
        {
            try
            {
                var employee = await _repository.Employee.FindById(x => x.UserId == user.Id);
                ProjectObjective myProjectObjective = new()
                {
                    ProjectId = projectObjective.ProjectId,
                    Description = projectObjective.Description,
                    Status = projectObjective.Status,
                    Percentage = projectObjective.Percentage,
                    CreatedBy = user.Id.ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = employee.UserId.ToString(),
                };
                await _repository.ProjectObjective.CreateAsync(myProjectObjective);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> UpdateProjectObjective(User user, ProjectObjective project, int Id)
        {
            try
            {

                var projectobjective = await _dbContext.ProjectObjective
                    .FirstOrDefaultAsync(x => x.Id == Id);

                if (projectobjective != null)
                {
                    projectobjective.Description = project.Description;
                    projectobjective.Status = project.Status;
                    projectobjective.Percentage = projectobjective.Percentage;
                    projectobjective.CreatedDate = projectobjective.CreatedDate;
                    projectobjective.CreatedBy = projectobjective.CreatedBy;
                    projectobjective.UpdatedDate = DateTime.Now;
                    projectobjective.UpdatedBy = user.Id.ToString();
                    await _repository.ProjectObjective.UpdateAsync(projectobjective);
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<UserStory>> GetUserStoryList(int projectId)
        {
            try
            {
                var UserStory = await _dbContext.UserStory.Where(x => x.ProjectId == projectId).OrderByDescending(x => x.Id).ToListAsync();
                return UserStory;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> AddUserStory(User user, UserStory UserStory)
        {
            try
            {

                UserStory MyUserStory = new UserStory()
                {
                    Name = UserStory.Name,
                    Description = UserStory.Description,
                    Status = UserStory.Status,
                    Percentage = UserStory.Percentage,
                    ProjectId = UserStory.ProjectId,
                    ProjectObjectiveId = UserStory.ProjectObjectiveId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.Id.ToString(),
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = user.Id.ToString(),
                };
                await _dbContext.UserStory.AddAsync(MyUserStory);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        public async Task<bool> UpdateUserStory(User user, UserStory UserStory)
        {
            try
            {
                var MyUserStory = await _dbContext.UserStory
                    .FirstOrDefaultAsync(x => x.Id == UserStory.Id);
                if (MyUserStory != null)
                {
                    MyUserStory.Name = UserStory.Name;
                    MyUserStory.Description = UserStory.Description;
                    MyUserStory.Status = UserStory.Status;
                    MyUserStory.Percentage = UserStory.Percentage;
                    MyUserStory.ProjectObjectiveId = UserStory.ProjectObjectiveId;
                    MyUserStory.StartDate = UserStory.StartDate;
                    MyUserStory.EndDate = UserStory.EndDate;
                    MyUserStory.UpdatedDate = DateTime.Now;
                    MyUserStory.UpdatedBy = UserStory.UpdatedBy;
                    _dbContext.UserStory.Update(MyUserStory);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddUserStoryUI(User user, UserStoryUI[] userStoryUI)
        {
            try
            {
                var result = _dbContext.UserStoryUI.Where(x => x.UserStoryId == userStoryUI[0].UserStoryId).ToList();
                _dbContext.UserStoryUI.RemoveRange(result);

                foreach (var obj in userStoryUI)
                {
                    var userstoryUI = new UserStoryUI()
                    {
                        UIId = obj.UIId,
                        UserStoryId = obj.UserStoryId,
                        CreatedBy = user.Id.ToString(),
                        UpdatedBy = user.Id.ToString(),
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };
                    await _dbContext.UserStoryUI.AddAsync(userstoryUI);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<UserStoryUI>> GetUserStoryUIList(int userStoryId)
        {
            try
            {
                var result = _dbContext.UserStoryUI.Where(x => x.UserStoryId == userStoryId).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AssignEmployeeProject(User user, EmployeeProject[] employeeProject)
        {
            var projempList = _dbContext.EmployeeProject.Where(x => x.ProjectId == employeeProject[0].ProjectId).ToList();

            foreach (var obj in employeeProject)
            {
                EmployeeProject emp = projempList.FirstOrDefault(x => x.EmployeeId == obj.EmployeeId && x.EndDate == null);

                if (emp == null)
                {
                    EmployeeProject employeeProject1 = new EmployeeProject()
                    {
                        ProjectId = obj.ProjectId,
                        EmployeeId = obj.EmployeeId,
                        StartDate = DateTime.Now,
                        CreatedBy = user.Id.ToString(),
                        UpdatedBy = user.Id.ToString(),
                        CreatedDate = DateTime.Now,

                    };
                    await _dbContext.EmployeeProject.AddAsync(employeeProject1);
                }
            }

            foreach (var obj in projempList)
            {
                var emp = employeeProject.FirstOrDefault(x => x.EmployeeId == obj.EmployeeId && obj.EndDate == null);

                if (emp == null)
                {
                    if (obj.EndDate == null)
                    {
                        obj.EndDate = DateTime.Now;
                        _dbContext.EmployeeProject.Update(obj);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignLead(User user, int userId, int projectId)
        {
            try
            {

                var result = await _dbContext.Lead.FirstOrDefaultAsync(x => x.UserId == userId);
                var empproject = await _dbContext.EmployeeProject.Where(x => x.ProjectId == projectId).ToListAsync();

                if (result == null)
                {
                    Lead lead = new Lead()
                    {
                        UserId = userId,
                        StartDate = DateTime.Now,
                        CreatedBy = user.Id.ToString(),
                        UpdatedBy = user.Id.ToString(),
                        CreatedDate = DateTime.Now,
                    };

                    var res = await _dbContext.Lead.AddAsync(lead);
                    await _dbContext.SaveChangesAsync();

                    foreach (var obj in empproject)
                    {
                        obj.LeadId = res.Entity.Id;
                    }

                    _dbContext.EmployeeProject.UpdateRange(empproject);
                }
                else
                {
                    foreach (var obj in empproject)
                    {
                        obj.LeadId = result.Id;
                    }

                    _dbContext.EmployeeProject.UpdateRange(empproject);

                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetUnAssignedProjects(int TeamID)
        {

            var Ids = _dbContext.TeamProject.Where(x => x.TeamId == TeamID).Select(x => x.ProjectId).ToList();
            var project = _dbContext.Project
                 .Where(x => !Ids.Contains(x.Id))
                .ToList();
            return project;
        }

        public async Task<ProjectStatDto> GetProjectStatDetails(int ProjectId)
        {
            try
            {
                var totalTask = await _repository.Task.FindByConditionAsync(x => x.ProjectId == ProjectId);
                var inProgressTask = await _repository.Task.FindByConditionAsync(x => x.ProjectId == ProjectId && x.Status == "In-Progress");
                var completedTask = await _repository.Task.FindByConditionAsync(x => x.ProjectId == ProjectId && x.Status == "Completed");
                var totalUserStory = await _repository.UserStory.FindByConditionAsync(x => x.ProjectId == ProjectId);
                var totalUI = await _repository.UserInterface.FindByConditionAsync(x => x.ProjectId == ProjectId);
                var totalProjObj = await _repository.ProjectObjective.FindByConditionAsync(x => x.ProjectId == ProjectId);
                var result = inProgressTask.ToList();

                var teamDetails = (from teamProj in _dbContext.TeamProject
                                   join team in _dbContext.Team on teamProj.TeamId equals team.Id
                                   where teamProj.ProjectId == ProjectId
                                   select new ProjectStatDto
                                   {
                                       TeamName = team.Name,
                                   }).FirstOrDefault();

                ProjectStatDto projStat = new()
                {
                    InProgressTask = inProgressTask.Count(),
                    TotalTask = totalTask.Count(),
                    CompletedTask = completedTask.Count(),
                    TotalUserStory = totalUserStory.Count(),
                    TotalUI = totalUI.Count(),
                    TotalProjectObjective = totalProjObj.Count(),
                    TeamName = teamDetails?.TeamName
                };
                // Calculate the percentage for each table based on the Percentage column
                projStat.TaskPercentage = totalTask.Count() != 0 ? totalTask.Sum(task => task.Percentage) / totalTask.Count() : 0;
                projStat.UserStoryPercentage = totalUserStory.Count() != 0 ? totalUserStory.Sum(x => x.Percentage) / totalUserStory.Count() : 0;
                projStat.UIPercentage = totalUI.Count() != 0 ? totalUI.Sum(x => x.Percentage) / totalUI.Count() : 0;
                projStat.ObjectivePercentage = totalProjObj.Count() != 0 ? totalProjObj.Sum(x => x.Percentage) / totalProjObj.Count() : 0;
                projStat.InProgressPercentage = inProgressTask.Count() != 0 ? inProgressTask.Sum(x => x.Percentage) / inProgressTask.Count() : 0;
                projStat.CompletedPercentage = completedTask.Count() != 0 ? completedTask.Sum(x => x.Percentage) / completedTask.Count() : 0;


                return projStat;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<Project>> ResourcesAssignedToAnyProject()
        {
            try
            {
                var projectList = await (from project in _dbContext.Project
                                         select new Project()
                                         {
                                             Id = project.Id,
                                             Name = project.Name,
                                             Type = project.Type,
                                             Description = project.Description,
                                             Status = project.Status,
                                             Percentage = project.Percentage,
                                             StartDate = project.StartDate,
                                             EndDate = project.EndDate,
                                             ProjectDocuments = _dbContext.ProjectDocuments.FirstOrDefault(x => x.ProjectId == project.Id),
                                             ProjectTechStacks = _dbContext.ProjectTechStack.Where(x => x.ProjectId == project.Id).ToList(),
                                             UserStoryCount = _dbContext.UserStory.Where(x => x.ProjectId == project.Id).Count(),
                                             //InProgressCount = _dbContext.Task.Count(x => x.Status == "Pending" && x.ProjectId == project.Id),
                                             TotalTaskCount = _dbContext.Task.Count(x => x.ProjectId == project.Id),
                                             NotStartedTaskCounts = _dbContext.Task.Count(x => x.ProjectId == project.Id && x.Status == "assigned"),
                                             UseInterfaceCount = _dbContext.UserInterface.Count(x => x.ProjectId == project.Id),
                                             TeamId = _dbContext.TeamProject.Where(x => x.ProjectId == project.Id && x.EndDate == null).Select(x => x.TeamId).FirstOrDefault(),
                                         }).OrderByDescending(x => x.Id).ToListAsync();

                return projectList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<ProjectDto> ResourcesAssignedByProject(int ProjectId)
        {
            try
            {
                var project = await (from p in _dbContext.Project
                                     where p.Id == ProjectId
                                     select new ProjectDto
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         Type = p.Type,
                                         Description = p.Description,
                                         Status = p.Status,
                                         Percentage = p.Percentage,
                                         StartDate = p.StartDate,
                                         EndDate = p.EndDate,
                                         ProjectDocuments = _dbContext.ProjectDocuments.FirstOrDefault(x => x.ProjectId == p.Id),
                                         ProjectTechStacks = _dbContext.ProjectTechStack.Where(x => x.ProjectId == p.Id).ToList(),
                                         UserStoryCount = _dbContext.UserStory.Count(x => x.ProjectId == p.Id),
                                         InProgressCount = _dbContext.Task.Count(x => x.Status == "Pending" && x.ProjectId == p.Id),
                                         TotalTaskCount = _dbContext.Task.Count(x => x.ProjectId == p.Id),
                                         NotStartedTaskCounts = _dbContext.Task.Count(x => x.ProjectId == p.Id && x.Status == "Assigned"),
                                         UseInterfaceCount = _dbContext.UserInterface.Count(x => x.ProjectId == p.Id),
                                         TeamId = _dbContext.TeamProject.Where(x => x.ProjectId == p.Id && x.EndDate == null).Select(x => x.TeamId).FirstOrDefault()
                                     }).FirstOrDefaultAsync();

                return project;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
