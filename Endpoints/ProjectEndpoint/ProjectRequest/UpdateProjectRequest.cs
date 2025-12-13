using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.ProjectEndpoint.ProjectRequest
{
    public class UpdateProjectRequest
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Company { get; set; }

        [MaxLength(500)]
        public string? InstagramUrl { get; set; }

        [MaxLength(500)]
        public string? FacebookUrl { get; set; }

        [MaxLength(500)]
        public string? LinkedinUrl { get; set; }

        [MaxLength(500)]
        public string? TwitterUrl { get; set; }
    }
}
