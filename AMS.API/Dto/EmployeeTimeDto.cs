namespace ProjectOversight.API.Dto
{
    public class EmployeeTimeDto
    {
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public string Comments { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Id { get; set; }
       
    }
}
