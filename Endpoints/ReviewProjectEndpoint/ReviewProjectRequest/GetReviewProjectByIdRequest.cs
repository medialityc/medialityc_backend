using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectRequest
{
    public class GetReviewProjectByIdRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
