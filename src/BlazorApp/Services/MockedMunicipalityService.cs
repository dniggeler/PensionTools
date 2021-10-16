using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services
{
    public class MockedMunicipalityService : IMunicipalityService
    {
        private readonly IConfiguration configuration;
        private readonly ILocalStorageService localStorageService;
        private readonly HttpClient httpClient;

        public MockedMunicipalityService(IConfiguration configuration, ILocalStorageService localStorageService, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.localStorageService = localStorageService;
            this.httpClient = httpClient;
        }

        public Task<IEnumerable<MunicipalityModel>> GetAllAsync()
        {
            string urlPath = configuration.GetSection("MunicipalityServiceUrl").Value;

            return httpClient.GetFromJsonAsync<IEnumerable<MunicipalityModel>>(urlPath);
        }

        public async Task<IEnumerable<TaxSupportedMunicipalityModel>> GetTaxSupportingAsync()
        {
            var cachedItems = await localStorageService.GetItemAsync<IEnumerable<TaxSupportedMunicipalityModel>>("TaxCalculatorServiceUrl");

            if (cachedItems is { })
            {
                return cachedItems;
            }

            string urlPath = configuration.GetSection("TaxCalculatorServiceUrl").Value;

            List<TaxSupportedMunicipalityModel> fetchedItems = await httpClient.GetFromJsonAsync<List<TaxSupportedMunicipalityModel>>(urlPath);

            if (fetchedItems is { })
            {
                await localStorageService.SetItemAsync("TaxCalculatorServiceUrl", fetchedItems);
                return fetchedItems;
            }

            return Enumerable.Empty<TaxSupportedMunicipalityModel>();
        }
    }
}
