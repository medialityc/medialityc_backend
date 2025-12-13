using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.AreaEndpoint.AreaRequest
{
    public class CreateAreaRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}
