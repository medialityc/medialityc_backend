using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileRequest
{
    public class UpdateWorkProfileRequest
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(150)]
        public string? FirsName { get; set; }

        [MaxLength(150)]
        public string? LastName { get; set; }

        public int? AreaId { get; set; }

        public int? RoleProfileId { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(300)]
        public string? GitHubProfile { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }

        [Range(0, 5)]
        public decimal? ReviewStars { get; set; }

        [MaxLength(2000)]
        public string? OverallReview { get; set; }
    }
}
