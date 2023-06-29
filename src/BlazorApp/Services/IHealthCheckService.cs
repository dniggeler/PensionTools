using System.Threading.Tasks;

namespace BlazorApp.Services;

public interface IHealthCheckService
{
    Task<bool> CheckAsync();
}
