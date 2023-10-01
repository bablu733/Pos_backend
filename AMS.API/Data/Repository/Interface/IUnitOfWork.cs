

namespace ProjectOversight.API.Data.Repository.Interface
{
    public interface IUnitOfWork 
    {
        IUserRepository User { get; }
        IEmployeeRepository Employee { get; }
        IEmployeeProjectRepository EmployeeProject { get; }       
        IProjectRepository Project { get; }
        IProjectTechStackRepository ProjectTechStack { get; }
        ICommentsRepository Comments { get; }
        IUserInterfaceRepository UserInterface { get; }
        IUserStoryRepository UserStory { get; }
        ITaskRepository Task { get; }
        IEmployeeTaskRepository EmployeeTask { get; }
        IUserStoryUIRepository UserStoryUI { get; }
        ICategoryRepository Category { get; }  
        IEmployeeDailyTaskRepository DailyTask { get; }
        IEmployeTimeRepository EmployeeTime { get; }
        ICommonMasterRepository CommonMaster { get; }
        ILogErrorRepository LogError { get; }
        ITeamRepository Team { get; }
        IProjectObjectiveRepository ProjectObjective { get; }
        ISkillsetRepository skillset { get; }
        IDayRepository day { get; }
        IEmployeeDayRepository employeeDay { get; }
        ITeamProjectRepository TeamProject { get; }
        IEmployeeGeoRepository EmployeeGeo { get; }
        ITaskCheckListRepository TaskCheckList { get; }
        Task<int> CommitAsync(CancellationToken token);
    }
}
