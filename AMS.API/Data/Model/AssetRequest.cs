namespace ProjectOversight.API.Data.Model
{
    public class AssetRequest:BaseEntity
    {
       
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string? TypeOfRequest { get; set; }

        public string? AssetType { get; set; }

        public string? Specification { get; set; }

        public string? Status { get; set; }

        public DateTime CreatedDate { get; set; } 

        public string? CreatedBy { get; set;}

        public DateTime? UpdatedDate { get; set; }

        public string UpdatedBy { get; set;}
    }
}
