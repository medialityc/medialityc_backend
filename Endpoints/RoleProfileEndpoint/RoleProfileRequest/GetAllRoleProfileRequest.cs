namespace Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest
{
    public class GetAllRoleProfileRequest
    {
        public string? Search { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SortBy { get; set; }
        public bool? IsDescending { get; set; }
    }
}
