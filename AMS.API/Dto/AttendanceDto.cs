using ProjectOversight.API.Data.Model;

namespace ProjectOversight.API.Dto
{
    public class  AttendenceVm
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? DayId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Department { get; set; }
        public List<EmployeeTime>? EmployeeTime { get; set; }
        public int InOutCount { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }

        public int TotalWorkHour { get; set; }
        public DateTime? Date { get; set; }
    }
     public class AttendanceDto
    {
        public int? EmployeeCount { get; set; }
        public int? Present { get; set; }
        public int? Absent { get; set; }
        public int onTime { get; set; }
        public int Late { get; set; }
        public List<AttendenceVm> Attendances { get; set; }

    }

}
