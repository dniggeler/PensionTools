using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PensionCoach.Tools.PostOpenApi.Models;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;

namespace PostOpenApi.Integration.Tests;

[Trait("Post OpenApi Tests", "Integration")]
public class PostOpenApiClientIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient client;

    public PostOpenApiClientIntegrationTests(WebApplicationFactory<Startup> factory)
    {
        client = factory.CreateDefaultClient(new Uri("http://localhost/api/data/municipality/"));
    }

    [Fact(DisplayName = "All Zip Codes")]
    public async Task Should_Get_All_Zip_Codes_Successfully()
    {
        IEnumerable<ZipModel> result = await client.GetFromJsonAsync<IEnumerable<ZipModel>>("zip") switch
        {
            {} a => a.ToList().OrderBy(item => item.BfsCode).ThenBy(item => item.MunicipalityName),
            null => Array.Empty<ZipModel>()
        };

        Snapshot.Match(result);
    }
}
