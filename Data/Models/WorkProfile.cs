using System.ComponentModel.DataAnnotations;
namespace Medialityc.Data.Models
{
    public class WorkProfile
    {
        public int Id { get; set; }
        [Required]
        public string FirsName { get; set; } = string.Empty;
        [Required]
        public string LastName {get;set; } = string.Empty;
        public required Area Area { get; set; } 
        public required RoleProfile Role { get; set; } 
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string GitHubProfile { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public decimal ReviewStars { get; set; }
        public string OverallReview { get; set; } = string.Empty;

        public ICollection<ReviewProject> Reviews { get; set; } = new List<ReviewProject>();


        
    }
}
