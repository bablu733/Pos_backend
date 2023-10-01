using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectOversight.API.Data.Model
{
    public class EmployeeAsset
    {
        public int Id { get; set; }

        [ForeignKey("EmployeeId")]
        public int EmployeeId { get; set; }

        [ForeignKey("AssetId")]
        public int AssetId { get; set; }

        public int AssetRequestId { get; set; }

        public string? EmployeeName { get; set; }

        public string? EmployeeCode { get; set; }

        public string? Department { get; set; }

        public DateTime? HandOverDate { get; set; }

        public string? HandOverBy { get; set; }

        public int AssetCode { get; set; }

        public string? AssetType { get; set; }

        public string? Specification { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set;}

        public string UpdatedBy { get; set;}
    }
}
