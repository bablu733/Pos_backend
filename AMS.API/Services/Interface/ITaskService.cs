using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Dto;
using Task = ProjectOversight.API.Data.Model.Task;

namespace ProjectOversight.API.Services.Interface
{
    public interface ITaskService
    {
        Task<List<TaskListDto>> GetTaskList();
        Task<List<Task>> GetTaskListValue();
        Task<Task> CreateTask(User user, TaskDTO task);
        Task<List<UserInterface>> GetUserInterfacelist(int UserStoryId);
        Task<List<UserStory>> GetProjectUSlist(int ProjectId);
        Task<List<TaskListDto>> GetEmployeeTaskList(int employeeId);
        Task<List<CommentsDto>> GetComments(int taskId);
        Task<Task> CreateTaskCheckList(User user, TaskCheckListDto CheckList);
        
    }
}
