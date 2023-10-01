namespace ProjectOversight.API.Dto
{
    public class TeamDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public int? LeadId { get; set; }
        public string Username { get; set; }
        public string? EmployeeName { get; set; }
        public decimal? AssignedHours { get; set; }
        public int? ProjectId { get; set; }

    }
}
