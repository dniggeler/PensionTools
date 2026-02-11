#nullable enable

using System.Net.Http.Json;
using System.Text.Json;
using Application.Municipality;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Municipality;

public class HttpMunicipalityConnector(
    IHttpClientFactory httpClientFactory,
    ILogger<HttpMunicipalityConnector> logger)
    : IMunicipalityConnector
{
    private const string ApiUrl = "https://swisstaxcalculator.estv.admin.ch/delegate/ost-integration/v1/lg-proxy/operation/c3b67379_ESTV/API_searchLocation";

    public async Task<IEnumerable<MunicipalityModel>> GetAllAsync()
    {
        logger.LogInformation("Fetching all municipalities via HTTP");
        
        var payload = new { Search = string.Empty };
        var response = await PostAsync<List<MunicipalityModel>>(payload);
        
        return response ?? new List<MunicipalityModel>();
    }

    public async Task<IEnumerable<MunicipalityModel>> SearchAsync(MunicipalitySearchFilter searchFilter)
    {
        logger.LogInformation("Searching municipalities with filter: {Name}", searchFilter.Name);
        
        var payload = new { Search = searchFilter.Name ?? string.Empty };
        var response = await PostAsync<List<MunicipalityModel>>(payload);
        
        if (response == null)
            return [];

        var filtered = response.AsEnumerable();

        if (searchFilter.Canton != Domain.Enums.Canton.Undefined)
        {
            filtered = filtered.Where(m => m.Canton == searchFilter.Canton);
        }

        if (searchFilter.YearOfValidity.HasValue)
        {
            filtered = filtered.Where(m => 
                !m.DateOfMutation.HasValue || 
                m.DateOfMutation.Value.Year > searchFilter.YearOfValidity.Value);
        }

        return filtered;
    }

    public async Task<Either<string, MunicipalityModel>> GetAsync(int bfsNumber, int year)
    {
        logger.LogInformation("Fetching municipality with BFS number {BfsNumber} for year {Year}", bfsNumber, year);
        
        var payload = new { Search = bfsNumber.ToString() };
        var response = await PostAsync<List<MunicipalityModel>>(payload);
        
        if (response == null || !response.Any())
        {
            return $"Municipality not found by BFS number {bfsNumber}";
        }

        var municipality = response.FirstOrDefault(m => m.BfsNumber == bfsNumber);
        
        if (municipality == null)
        {
            return $"Municipality not found by BFS number {bfsNumber}";
        }

        return municipality;
    }

    public async Task<IReadOnlyCollection<TaxSupportedMunicipalityModel>> GetAllSupportTaxCalculationAsync()
    {
        logger.LogInformation("Fetching all municipalities supporting tax calculation");
        
        var payload = new { Search = string.Empty };
        var response = await PostAsync<List<MunicipalityModel>>(payload);
        
        if (response == null)
            return Array.Empty<TaxSupportedMunicipalityModel>();

        var supportedMunicipalities = response
            .Where(m => m.EstvTaxLocationId.HasValue)
            .Select(m => new TaxSupportedMunicipalityModel
            {
                BfsMunicipalityNumber = m.BfsNumber,
                Name = m.Name,
                Canton = m.Canton,
                MaxSupportedYear = DateTime.Now.Year,
                EstvTaxLocationId = m.EstvTaxLocationId
            })
            .ToList();

        return supportedMunicipalities;
    }

    private async Task<T?> PostAsync<T>(object payload)
    {
        try
        {
            using var httpClient = httpClientFactory.CreateClient();
            
            var response = await httpClient.PostAsJsonAsync(ApiUrl, payload);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<T>();
            return result;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed while calling municipality API");
            throw;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize response from municipality API");
            throw;
        }
    }
}
