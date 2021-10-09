using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorApp.MyComponents;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes;

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
        public async Task<IEnumerable<MunicipalityViewModel>> GetAllAsync()
        {
            string urlPath = configuration.GetSection("MunicipalityServiceUrl").Value;

            var response = await httpClient.GetFromJsonAsync<IEnumerable<MunicipalityModel>>(urlPath);

            return response.Select(item => new MunicipalityViewModel
            {
                Name = item.Name,
                BfsNumber = item.BfsNumber,
                Canton = item.Canton,
                MutationId = item.MutationId,
                DateOfMutation = item.DateOfMutation,
                SuccessorId = item.SuccessorId
            });
        }
    }
}
