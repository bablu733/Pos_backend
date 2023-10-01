using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Dto;

namespace ProjectOversight.API.Services.Interface
{
    public interface IEmployeeDailyTaskService
    {
        Task<List<EmployeeDailyTaskDto>> GetTimePlanList();
        Task<IEnumerable<EmployeeTaskListDto>> GetEmployeeTask(int EmployeeId);
        Task<EmployeeDailyTask> AddEmployeeDailyTask(User user, EmployeeDailyTaskDto employeeDailyTask);
        Task<List<EmployeeDailyTaskDto>> GetEmployeeTimePlanList(int employeeId);
        Task<List<CommentsDto>> GetComments();
        Task<EmployeeDailyTask> GetEmployeeDailyTaskById(int employeeId, int projectId);
        Task<List<EmployeeDailyTask>> GetEmployeeDailyTask(int employeeTaskId);
        Task<List<EmployeeTaskDto>> GetCompletedWhatsapptaskListByTaskId(int employeeId, DateTime WeekEndingDate);

    }
}
