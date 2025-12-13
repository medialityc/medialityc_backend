namespace Medialityc.Data.Models
{
    public class ReviewProject
    {
        public int Id { get; set; }

        public Project Project { get; set; } = new Project();
        public int ProjectId { get; set; }

        public WorkProfile? WorkProfile { get; set; } 
        public int WorkProfileId { get; set;}

        public string SpecificReview { get; set; } = string.Empty;
        public decimal PerformanceEvaluation { get; set; }
        
    }
}