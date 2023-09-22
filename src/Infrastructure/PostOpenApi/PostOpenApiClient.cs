using System.Net.Http.Json;
using Infrastructure.PostOpenApi.Models;

namespace Infrastructure.PostOpenApi
{
    public class PostOpenApiClient : IPostOpenApiClient
    {
        internal static string ClientName = "PostOpenApiClient";

        private readonly IHttpClientFactory httpClientFactory;

        public PostOpenApiClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public Task<OpenApiZipInfo> GetZipCodesAsync(int limit, int offset)
        {
            string[] fieldNames = { "postleitzahl","plz_zz", "gilt_ab_dat", "bfsnr", "kanton", "ortbez27" } ;

            HttpClient client = httpClientFactory.CreateClient(ClientName);

            string partFields = string.Join("%2C%20", fieldNames);

            return client.GetFromJsonAsync<OpenApiZipInfo>(
                $"plz_verzeichnis_v2/records?select={partFields}&limit={limit}&offset={offset}&timezone=UTC");
        }
    }
}
