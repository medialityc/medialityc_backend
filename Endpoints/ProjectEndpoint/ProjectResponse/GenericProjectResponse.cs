using System;

namespace Medialityc.Endpoints.ProjectEndpoint.ProjectResponse
{
    public class GenericProjectResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public NetworkResponseForProject Network { get; set; } = new NetworkResponseForProject();
        public DateTime CreatedAt { get; set; }
    }
    public class NetworkResponseForProject{
        public int Id { get; set;}
        public string Instagram { get; set; } = string.Empty;
        public string LinkedIn { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string Twitter { get; set; } = string.Empty;

    }
}
