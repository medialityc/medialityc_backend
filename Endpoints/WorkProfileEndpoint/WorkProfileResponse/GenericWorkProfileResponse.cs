namespace Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileResponse
{
    public class GenericWorkProfileResponse
    {
        public int Id { get; set; }
        public string FirsName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int AreaId { get; set; }
        public int RoleProfileId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string GitHubProfile { get; set; } = string.Empty;
        public string? Image { get; set; }
        public decimal ReviewStars { get; set; }
        public string? OverallReview { get; set; }
    }
}
