using Infrastructure.PostOpenApi.Models;

namespace Infrastructure.PostOpenApi;

public interface IPostOpenApiClient
{
    Task<OpenApiZipInfo> GetZipCodesAsync(int limit, int offset);
}
