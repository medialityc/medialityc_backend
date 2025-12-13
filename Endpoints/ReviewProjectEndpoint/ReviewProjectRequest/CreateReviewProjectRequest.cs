using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectRequest
{
    public class CreateReviewProjectRequest
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int WorkProfileId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string SpecificReview { get; set; } = string.Empty;

        [Range(0, 5)]
        public decimal PerformanceEvaluation { get; set; }
    }
}
