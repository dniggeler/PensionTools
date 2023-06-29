using System.Threading.Tasks;

namespace BlazorApp.Services.Mock;

public class MockedHealthCheck : IHealthCheckService
{
    public Task<bool> CheckAsync()
    {
        return Task.FromResult(true);
    }
}
