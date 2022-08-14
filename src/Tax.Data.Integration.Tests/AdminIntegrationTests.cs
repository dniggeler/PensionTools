using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.PostOpenApi.Models;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;
using Xunit.Abstractions;

namespace Tax.Data.Integration.Tests;

[Trait("Admin", "Integration")]
public class AdminIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly ITestOutputHelper outputHelper;
    private readonly HttpClient client;

    public AdminIntegrationTests(ITestOutputHelper outputHelper)
    {
        this.outputHelper = outputHelper;
        var testServer = new TestServer(
            new WebHostBuilder()
                .ConfigureAppConfiguration((_, builder) =>
                {
                    builder.AddJsonFile("appsettings.integration.json");
                })
                .UseStartup<Startup>());

        client = testServer.CreateClient();
        client.BaseAddress = new Uri("http://localhost/api/admin/");
    }

    [Fact(DisplayName = "Populate With Tax Location")]
    public async Task Populate_With_Tax_Location()
    {
        bool doClear = true;

        HttpResponseMessage response = await client.PostAsync($"tax/populate/{doClear}", null);

        response.EnsureSuccessStatusCode();

        int result = await response.Content.ReadFromJsonAsync<int>();

        outputHelper.WriteLine($"Number of updates: {result}");
    }

    [Fact(DisplayName = "Stage Zip Codes")]
    public async Task Stage_With_ZipCodes()
    {
        HttpResponseMessage response = await client.PostAsync("zip/stage", null);

        response.EnsureSuccessStatusCode();

        int result = await response.Content.ReadFromJsonAsync<int>();

        Assert.True(result > 0);
    }

    [Fact(DisplayName = "All Zip Codes")]
    public async Task Get_All_Zip_Codes_Successfully()
    {
        IEnumerable<ZipModel> result = await client.GetFromJsonAsync<IEnumerable<ZipModel>>("zip") switch
        {
            { } a => a.ToList().OrderBy(item => item.BfsCode).ThenBy(item => item.MunicipalityName),
            null => Array.Empty<ZipModel>()
        };

        Snapshot.Match(result);
    }
}
