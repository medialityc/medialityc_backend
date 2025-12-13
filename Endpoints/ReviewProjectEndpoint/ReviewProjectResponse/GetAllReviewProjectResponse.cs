namespace Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectResponse
{
    public class GetAllReviewProjectResponse
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int WorkProfileId { get; set; }
        public string SpecificReview { get; set; } = string.Empty;
        public decimal PerformanceEvaluation { get; set; }
    }
}
