using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest
{
    public class CreateRoleProfileRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}
