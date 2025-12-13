using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileRequest
{
    public class GetWorkProfileByIdRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
