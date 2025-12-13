using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.ProjectEndpoint.ProjectRequest
{
    public class DeleteProjectRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
