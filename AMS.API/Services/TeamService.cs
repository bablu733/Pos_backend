using Microsoft.EntityFrameworkCore;
using ProjectOversight.API.Data;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Services.Interface;

namespace ProjectOversight.API.Services
{
    public class TeamService : ITeamService
    {

        private readonly ProjectOversightContext _dbContext;
        private readonly IUnitOfWork _repository;
        public TeamService(ProjectOversightContext dbContext, IUnitOfWork repository)
        {

            _dbContext = dbContext;
            _repository = repository;
        }

        public async Task<List<Team>> GetTeamList()
        {
            try
            {
                var teamList = await _dbContext.Team.OrderByDescending(x => x.Id).ToListAsync();
                return teamList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TeamEmployee>> GetTeamMemberList()
        {
            try
            {
                var teamList = await _dbContext.TeamEmployee.Include(o => o.Employee).Include(o => o.Employee.User).ToListAsync();
                return teamList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Team> GetTeamById(int Id)
        {
            try
            {
                var TeamList = await _dbContext.Team.Where(x => x.Id == Id).FirstOrDefaultAsync();

                return TeamList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<TeamDto>> GetTeamNames(int projectId)
        {
            var query = await (
                               from employeeProject in _dbContext.EmployeeProject
                               join employee in _dbContext.Employee on employeeProject.EmployeeId equals employee.Id
                               join user in _dbContext.Users on employee.UserId equals user.Id
                               where employeeProject.ProjectId == projectId && employeeProject.EndDate == null
                               select new TeamDto
                               {
                                   EmployeeId = employeeProject.EmployeeId,
                                   UserId = employee.UserId,
                                   Username = user.Name,
                                   LeadId = _dbContext.Lead.FirstOrDefault(x => x.Id == employeeProject.LeadId).UserId,
                               }).OrderBy(x => x.Username).ToListAsync();
            return query;
        }

        public async Task<List<TeamDto>> GetTeamEmployeelist(int teamId)
        {
            var query = await (from team in _dbContext.Team
                               join teamEmployee in _dbContext.TeamEmployee on team.Id equals teamEmployee.TeamId
                               join employee in _dbContext.Employee on teamEmployee.EmployeeId equals employee.Id
                               join user in _dbContext.Users on employee.UserId equals user.Id
                               where team.Id == teamId && teamEmployee.EndDate == null
                               select new TeamDto
                               {
                                   TeamId = team.Id,
                                   TeamName = team.Name,
                                   EmployeeId = teamEmployee.EmployeeId,
                                   EmployeeName = user.Name
                               }
                ).OrderBy(x => x.EmployeeName).ToListAsync();
            return query;
        }

        public async Task<bool> AssignEmployeeToTeam(TeamEmployee[] teamEmployee)
        {
            var teamempList = _dbContext.TeamEmployee.Where(x => x.TeamId == teamEmployee[0].TeamId).ToList();

            foreach (var obj in teamEmployee)
            {
                TeamEmployee emp = teamempList.FirstOrDefault(x => x.EmployeeId == obj.EmployeeId && x.EndDate == null);

                if (emp == null)
                {
                    TeamEmployee teamEmployee1 = new TeamEmployee()
                    {
                        TeamId = obj.TeamId,
                        EmployeeId = obj.EmployeeId,
                        LeadId = teamempList?.FirstOrDefault()?.LeadId,
                        StartDate = DateTime.Now,
                        CreatedBy = obj.CreatedBy,
                        UpdatedBy = obj.UpdatedBy,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,

                    };
                    await _dbContext.TeamEmployee.AddAsync(teamEmployee1);
                }
            }

            foreach (var obj in teamempList)
            {
                var emp = teamEmployee.FirstOrDefault(x => x.EmployeeId == obj.EmployeeId && obj.EndDate == null);

                if (emp == null)
                {
                    if (obj.EndDate == null)
                    {
                        obj.EndDate = DateTime.Now;
                        _dbContext.TeamEmployee.Update(obj);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> AddTeam(User user, Team Team)
        {
            try
            {
                var employee = await _repository.Employee.FindById(x => x.UserId == user.Id);
                Team Teams = new Team()
                {
                    Name = Team.Name,
                    StartDate = Team.StartDate,
                    EndDate = Team.EndDate,
                    CreatedDate = DateTime.Now,
                    CreatedBy = employee.UserId.ToString(),
                    UpdatedBy = employee.UserId.ToString(),
                    UpdatedDate = DateTime.Now,
                };
                var TeamDetails = await _dbContext.Team.AddAsync(Teams);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        public async Task<bool> Updateteam(User user, Team team)
        {
            try
            {
                var existingTeam = await _dbContext.Team.FindAsync(team.Id);
                if (existingTeam == null)
                {
                    return false;
                }

                existingTeam.Name = team.Name;
                existingTeam.StartDate = team.StartDate;
                existingTeam.EndDate = DateTime.Now;
                existingTeam.CreatedDate = DateTime.Now;
                existingTeam.UpdatedDate = DateTime.Now;
                existingTeam.UpdatedBy = team.UpdatedBy;

                _dbContext.Entry(existingTeam).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> AddTeamObjective(User user, TeamObjective teamObjective)
        {
            try
            {
                var employee = await _repository.Employee.FindById(x => x.UserId == user.Id);
                TeamObjective myTeamObjective = new()
                {
                    TeamId = teamObjective.TeamId,
                    Description = teamObjective.Description,
                    Status = teamObjective.Status,
                    Percentage = teamObjective.Percentage,
                    CreatedDate = DateTime.Now,
                    CreatedBy = employee.UserId.ToString(),
                    UpdatedBy = employee.UserId.ToString(),

                };

                _dbContext.TeamObjective.Add(myTeamObjective);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> UpdateTeamObjective(User user, TeamObjective updatedObjective)
        {
            try
            {
                var employee = await _repository.Employee.FindById(x => x.UserId == user.Id);
                var teamObjective = await _dbContext.TeamObjective.FindAsync(updatedObjective.Id);
                if (teamObjective != null)
                {
                    teamObjective.Id = updatedObjective.Id;
                    teamObjective.TeamId = teamObjective.TeamId;
                    teamObjective.Description = updatedObjective.Description;
                    teamObjective.Status = updatedObjective.Status;
                    teamObjective.Percentage = teamObjective.Percentage;
                    teamObjective.UpdatedDate = DateTime.Now;
                    teamObjective.UpdatedBy = employee.UserId.ToString();
                    await _dbContext.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<TeamObjective>> GetTeamObjectiveList(int teamId)
        {
            try
            {
                var teamObjective = await _dbContext.TeamObjective.Where(x => x.TeamId == teamId).ToListAsync();
                return teamObjective;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TeamObjective> GetTeamObjectiveById(int Id)
        {
            try
            {
                var teamObjective = await _dbContext.TeamObjective.SingleOrDefaultAsync(x => x.Id == Id);
                return teamObjective;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TeamProject>> GetTeamProjectList(int teamId)
        {
            try
            {
                var result = (from teamProject in _dbContext.TeamProject
                              join project in _dbContext.Project on teamProject.ProjectId equals project.Id
                              where teamProject.TeamId == teamId && teamProject.EndDate==null
                              select new TeamProject()
                              {
                                  ProjectName = project.Name,
                                  TeamId = teamProject.TeamId,
                                  ProjectId = teamProject.ProjectId,
                              }
                          ).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> AddTeamProject(User user, TeamProject[] teamProject)
        {
            try
            {
                var teamProjectList = _dbContext.TeamProject.Where(x => x.TeamId == teamProject[0].TeamId).ToList();

            foreach (var obj in teamProject)
            {
                TeamProject emp = teamProjectList.FirstOrDefault(x => x.ProjectId == obj.ProjectId && x.EndDate == null);

                if (emp == null)
                {
                    TeamProject employeeProject1 = new TeamProject()
                    {
                        ProjectId = obj.ProjectId,
                        TeamId = obj.TeamId,
                        StartDate = DateTime.Now,
                        CreatedBy = user.Id.ToString(),
                        UpdatedBy = user.Id.ToString(),
                        CreatedDate = DateTime.Now,

                    };
                    await _dbContext.TeamProject.AddAsync(employeeProject1);
                }
            }

            foreach (var obj in teamProjectList)
            {
                var teaproj = teamProject.FirstOrDefault(x => x.ProjectId == obj.ProjectId && obj.EndDate == null);

                if (teaproj == null)
                {
                    if (obj.EndDate == null)
                    {
                        obj.EndDate = DateTime.Now;
                        _dbContext.TeamProject.Update(obj);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<EmployeeTask>> GetTeamEmployeeTaskList(int Id)
        {
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(7);
            var tasks = await _dbContext.EmployeeTask.Where(x => x.EmployeeId == Id && x.EstStartDate >= startDate && x.EstStartDate <= endDate).ToListAsync();

            return tasks;
        }
      

        public async Task<List<Project>> GetProjectList(int teamId)
        {
            try
            {
                var result = (from Project in _dbContext.Project
                              join TeamProject in _dbContext.TeamProject on Project.Id equals TeamProject.ProjectId
                              where TeamProject.TeamId == teamId
                              select new Project()
                              {
                                  Name = Project.Name,
                                  Type = Project.Type,
                                  Description= Project.Description,
                                  Percentage = Project.Percentage,
                                  Id = Project.Id,
                                  TeamId = TeamProject.TeamId
                              }
                          ).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}

