using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectRequest
{
    public class UpdateReviewProjectRequest
    {
        [Required]
        public int Id { get; set; }

        public int? ProjectId { get; set; }

        public int? WorkProfileId { get; set; }

        [MaxLength(2000)]
        public string? SpecificReview { get; set; }

        [Range(0, 5)]
        public decimal? PerformanceEvaluation { get; set; }
    }
}
