using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileRequest
{
    public class DeleteWorkProfileRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
