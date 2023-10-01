using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Data;
using ProjectOversight.API.Services.Interface;
using ProjectOversight.API.Constants;
using System.Linq;

namespace ProjectOversight.API.Services
{
    public class ReleaseNotesService : IReleaseNotesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ProjectOversightContext _dbContext;

        public ReleaseNotesService(IUnitOfWork repository, IMapper mapper, UserManager<User> userManager,
            RoleManager<Role> roleManager, IConfiguration configuration, ProjectOversightContext dbContext)
        {
            _mapper = mapper;
            _userManager = userManager;
            _repository = repository;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }
        public async Task<List<Data.Model.Task>> GetAllReadyForUATTasklist(int projectId)
        {
            try
            {
                var projectList = await _repository.Task.FindByConditionAsync(x => x.ProjectId == projectId &&x.Status == Status.ReadyForUAT || x.Status == "Ready For UAT");
                var result = projectList.ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<Data.Model.Task>> UpdateInUATTask(List<int> projectId)
        {
            try
            {
                const string InUATStatus = "IN-UAT";

                // Update Task table
                var taskList = await _repository.Task.FindByConditionAsync(x => projectId.Contains(x.Id) &&(x.Status == Status.ReadyForUAT || x.Status == "Ready For UAT"));
                var taskLists = taskList.Select(x=>x.Id).ToList();
                foreach (var task in taskList)
                {
                    task.Status = InUATStatus;
                   await _repository.Task.UpdateAsync(task);
                }
                var employeeDailyList = await _repository.EmployeeTask.FindByConditionAsync(x => taskLists.Contains(x.TaskId));
                var employeeDailyLists = employeeDailyList.Select(x => x.Id).ToList();
                foreach (var employeeTask in employeeDailyList)
                {
                    employeeTask.Status = InUATStatus;
                  await  _repository.EmployeeTask.UpdateAsync(employeeTask);
                }
                var employeeTaskList = await _repository.DailyTask.FindByConditionAsync(x => employeeDailyLists.Contains(x.EmployeeTaskId));
                foreach (var dailyTask in employeeTaskList)
                {
                    dailyTask.Status = InUATStatus;
                   await _repository.DailyTask.UpdateAsync(dailyTask);
                }
                return taskList.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
