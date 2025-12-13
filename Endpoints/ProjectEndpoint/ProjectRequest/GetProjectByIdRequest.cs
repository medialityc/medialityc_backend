using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.ProjectEndpoint.ProjectRequest
{
    public class GetProjectByIdRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
