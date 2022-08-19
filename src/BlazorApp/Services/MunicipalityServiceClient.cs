using System.Collections.Generic;
using System.IO;
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
    public class MunicipalityServiceClient : IMunicipalityService
    {
        private readonly IConfiguration configuration;
        private readonly ILocalStorageService localStorageService;
        private readonly HttpClient httpClient;

        public MunicipalityServiceClient(IConfiguration configuration, ILocalStorageService localStorageService, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.localStorageService = localStorageService;
            this.httpClient = httpClient;
        }
        public async Task<IEnumerable<MunicipalityModel>> GetAllAsync()
        {
            string urlPath = configuration.GetSection("MunicipalityServiceUrl").Value;

            var response = await httpClient.PostAsJsonAsync(Path.Combine(urlPath, "search"), GetFilter());

            response.EnsureSuccessStatusCode();

            IEnumerable<MunicipalityModel> result =
                await response.Content.ReadFromJsonAsync<IEnumerable<MunicipalityModel>>();

            return result?.Select(item => new MunicipalityModel
            {
                Name = item.Name,
                BfsNumber = item.BfsNumber,
                Canton = item.Canton,
                MutationId = item.MutationId,
                DateOfMutation = item.DateOfMutation,
                SuccessorId = item.SuccessorId,
                EstvTaxLocationId = item.EstvTaxLocationId
            });


            static MunicipalitySearchFilter GetFilter()
            {
                return new MunicipalitySearchFilter
                {
                    YearOfValidity = 2021
                };
            }
        }

        public async Task<IEnumerable<TaxSupportedMunicipalityModel>> GetTaxSupportingAsync()
        {
            var cachedItems = await localStorageService.GetItemAsync<IEnumerable<TaxSupportedMunicipalityModel>>("TaxCalculatorServiceUrl");

            if (cachedItems is { })
            {
                return cachedItems;
            }

            string urlPath = configuration.GetSection("TaxCalculatorServiceUrl").Value;

            List<TaxSupportedMunicipalityModel> fetchedItems =
                await httpClient.GetFromJsonAsync<List<TaxSupportedMunicipalityModel>>(Path.Combine(urlPath, "municipality"));

            if (fetchedItems is { })
            {
                await localStorageService.SetItemAsync("TaxCalculatorServiceUrl", fetchedItems);
                return fetchedItems;
            }

            return Enumerable.Empty<TaxSupportedMunicipalityModel>();
        }
    }
}
