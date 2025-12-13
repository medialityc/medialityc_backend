using System;

namespace Medialityc.Endpoints.ProjectEndpoint.ProjectResponse
{
    public class GetAllProjectResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public NetworkResponseForProject Network { get; set; } = new NetworkResponseForProject();
    }
}
