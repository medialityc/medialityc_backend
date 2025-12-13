using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectRequest
{
    public class DeleteReviewProjectRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
