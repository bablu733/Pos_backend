using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Dto;
using AutoMapper;

namespace ProjectOversight.API.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProjectOversight.API.Data.Model.Task, TaskDTO>().ReverseMap();
            CreateMap<EmployeeTask, EmployeeTaskDto>().ReverseMap();
            CreateMap<UserStoryUI, TaskDTO>().ReverseMap();
            CreateMap<UserStoryUI,EmployeeTaskDto>().ReverseMap();
            CreateMap<EmployeeDailyTask, EmployeeDailyTaskDto>().ReverseMap();
            CreateMap<Asset, AssetDTO>().ReverseMap();
            CreateMap<AssetRequest, AssetCodeDTO>().ReverseMap();
            CreateMap<ProjectOversight.API.Data.Model.Task, TaskCheckListDto>().ReverseMap();
            //CreateMap<TimePlan, EmpWorkHistoryDto>().ReverseMap();
        }
    }
}
