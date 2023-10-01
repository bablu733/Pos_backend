using ProjectOversight.API.Data.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectOversight.API.Dto
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        [Range(0, 100)]
        public int Percentage { get; set; }
        public virtual ProjectDocuments? ProjectDocuments { get; set; }
        public virtual ICollection<ProjectTechStack>? ProjectTechStacks { get; set; }

        [NotMapped]
        public int[]? TechStackId { get; set; }
        [NotMapped]
        public int UserStoryCount { get; set; }
        [NotMapped]
        public int UseInterfaceCount { get; set; }
        [NotMapped]
        public int TotalTaskCount { get; set; }
        [NotMapped]
        public int InProgressCount { get; set; }
        [NotMapped]
        public int NotStartedTaskCounts { get; set; }

        [NotMapped]
        public int TeamId { get; set; }
    }
}
