using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.AreaEndpoint.AreaRequest
{
    public class GetAreaByIdRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
