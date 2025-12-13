using System.ComponentModel.DataAnnotations;
namespace Medialityc.Data.Models{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Company { get; set; } = string.Empty;

        public Network Network { get; set; } = new Network();

        public DateTime CreatedAt { get; set; }

        public ICollection<ReviewProject> Reviews { get; set; } = new List<ReviewProject>();
    }
}