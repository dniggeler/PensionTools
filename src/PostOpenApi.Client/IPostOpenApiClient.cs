using PensionCoach.Tools.PostOpenApi.Models;

namespace PensionCoach.Tools.PostOpenApi;

public interface IPostOpenApiClient
{
    Task<OpenApiZipInfo> GetZipCodesAsync(int limit, int offset);
}
