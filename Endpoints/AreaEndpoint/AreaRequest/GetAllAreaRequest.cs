using System.ComponentModel.DataAnnotations;

namespace Medialityc.Endpoints.AreaEndpoint.AreaRequest
{
    public class GetAllAreaRequest
    {
        [Range(1, int.MaxValue)]
        public int? Page { get; set; }

        [Range(1, 200)]
        public int? PageSize { get; set; }

        public string? Search { get; set; }

        public string? SortBy { get; set; }

        public bool? IsDescending { get; set; }
    }
}
