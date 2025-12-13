using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest
{
    public class GetRoleProfileByIdRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
