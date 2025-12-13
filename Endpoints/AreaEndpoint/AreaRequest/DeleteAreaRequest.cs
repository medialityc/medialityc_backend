using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.AreaEndpoint.AreaRequest
{
    public class DeleteAreaRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
