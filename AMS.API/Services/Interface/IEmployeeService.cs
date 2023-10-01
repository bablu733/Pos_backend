using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Dto;

namespace ProjectOversight.API.Services.Interface
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetEmployeeList();
        Task<EmployeeStatDto> GetEmployeeStatDetails(User user);
        Task<bool> UpdateEmployee(UserCreateDto userCreateDto);
        Task<bool> AssignSkill(EmployeeSkillSet[] empSkillSet);
        Task<List<EmployeeSkillSet>> GetEmployeeSkillById(int employeeId);
        Task<List<TeamDto>> GetEmployeeTask(int teamId,DateTime? weekend);
        Task<List<EmployeeProject>> GetEmployeeProject(int projectId);
        Task<AttendanceDto> GetEmployeeAttendance(DateTime selectedDate);
        Task<EmployeeAttendanceVM> GetAttendanceByEmployeeId(int userId);
        Task<EmployeeTaskVM> GetEmployeeTasks(int employeeId);
        Task<Employee> UpdateByEmployeeId(int userId,EmployeeDto employeeDto);


    }
}
