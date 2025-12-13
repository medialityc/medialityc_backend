using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest
{
    public class DeleteRoleProfileRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
