using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileRequest
{
    public class CreateWorkProfileRequest
    {
        [Required]
        [MaxLength(150)]
        public string FirsName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public int AreaId { get; set; }

        [Required]
        public int RoleProfileId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string GitHubProfile { get; set; } = string.Empty;

        public required IFormFile Image { get; set; }

        [Range(0, 5)]
        public decimal ReviewStars { get; set; }

        [MaxLength(2000)]
        public string? OverallReview { get; set; }
    }
}
