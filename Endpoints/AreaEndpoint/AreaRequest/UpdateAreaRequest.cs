using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.AreaEndpoint.AreaRequest
{
    public class UpdateAreaRequest
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }
    }
}
