using FastEndpoints;

namespace RedmediaTyc.Endpoint
{
    public class Hello : EndpointWithoutRequest<string>
    {
        public override void Configure()
        {
            Get("/hello");
            AllowAnonymous();
        }

        public override async Task<string> 
        ExecuteAsync(CancellationToken ct)
        {
            return "Hello";
        }
    }
}
