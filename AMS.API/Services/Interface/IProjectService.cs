using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Dto;
using Task = ProjectOversight.API.Data.Model.Task;

namespace ProjectOversight.API.Services.Interface
{
    public interface IProjectService
    {
        Task<List<Project>> GetEmployeeProjectlist(User user);
        Task<List<Project>> GetAllProjectlist();
        Task<List<UserInterface>> GetUserInterfacelist(int UserStoryId);
        Task<List<UserStory>> GetProjectUSlist(int ProjectId);        
        Task<EmployeeTask> CreateEmployeeDayPlan(User user, EmployeeTaskDto task);
        Task<List<Category>> GetCategoriesList();
        Task<List<ProjectDto>> GetProjectlist();
        Task<Project> GetProjectById(int Id);
        Task<bool> AddEmployeeProject(User user, EmployeeProjectDto employeeProject);
        Task<bool> AddProject(User user, Project project);
        Task<bool> UpdateProject(User user, Project project,int Id);
        Task<List<ProjectObjective>> GetProjectObjective(int ProjectId);
        Task<ProjectObjective> GetProjectObjectiveById(int Id);       
        Task<bool> AddProjectObjective(User user, ProjectObjective projectObjective);
        Task<bool> UpdateProjectObjective(User user, ProjectObjective updatedProjectObjective, int id);
        Task<List<UserStory>> GetUserStoryList(int projectId);
        Task<bool> AddUserStory(User user, UserStory UserStory);
        Task<bool> UpdateUserStory(User user, UserStory UserStory);
        Task<bool> AddUserStoryUI(User user, UserStoryUI[] userStoryUI);
        Task<List<UserStoryUI>> GetUserStoryUIList(int userStoryId);
        Task<bool> AddUserInterface(User user,UserInterface userInterface);
        Task<bool> UpdateUserInterface(UserInterface userInterface);
        Task<bool> AssignEmployeeProject(User user, EmployeeProject[] employeeProject);
        Task<bool> AssignLead(User user,int userId, int projectId);
        Task<List<Project>> GetUnAssignedProjects(int TeamId);
        //Task<ProjectObjective> FindByIdAsync(int projectId);
        Task<bool> Updatetask(User user, EmployeeTaskDto dayPlan);
        Task<ProjectStatDto> GetProjectStatDetails(int ProjectId);
        Task<List<Project>> ResourcesAssignedToAnyProject();
        Task<ProjectDto> ResourcesAssignedByProject(int ProjectId);
    }
}
