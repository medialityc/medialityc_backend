using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest
{
    public class UpdateRoleProfileRequest
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }
    }
}
