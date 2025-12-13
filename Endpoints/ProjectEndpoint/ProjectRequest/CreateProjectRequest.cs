using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.ProjectEndpoint.ProjectRequest
{
    public class CreateProjectRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Company { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string InstagramUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FacebookUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string LinkedinUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string TwitterUrl { get; set; } = string.Empty;
    }
}
