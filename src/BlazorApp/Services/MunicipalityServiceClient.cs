using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services
{
    public class MunicipalityServiceClient : IMunicipalityService
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public MunicipalityServiceClient(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
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
                SuccessorId = item.SuccessorId
            });


            MunicipalitySearchFilter GetFilter()
            {
                return new MunicipalitySearchFilter
                {
                    YearOfValidity = 2021
                };
            }
        }

        public async Task<IEnumerable<TaxSupportedMunicipalityModel>> GetTaxSupportingAsync()
        {
            string urlPath = configuration.GetSection("TaxCalculatorServiceUrl").Value;

            return await httpClient.GetFromJsonAsync<IEnumerable<TaxSupportedMunicipalityModel>>(Path.Combine(urlPath, "municipality"));
        }
    }
}
