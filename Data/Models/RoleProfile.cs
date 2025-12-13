using System.ComponentModel.DataAnnotations;
namespace Medialityc.Data.Models
{
    public class RoleProfile
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}