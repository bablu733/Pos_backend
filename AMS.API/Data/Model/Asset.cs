

namespace ProjectOversight.API.Data.Model
{
    public class Asset:BaseEntity
    {
        public int Id { get; set; }
        public int AssetCode { get; set; }
        public string? AssetType { get; set; }
        public string? Specification { get; set; }
        public string Description { set; get; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
