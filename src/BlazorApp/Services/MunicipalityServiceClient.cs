using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorApp.MyComponents;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes.Municipality;

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

            try
            {
                var response = await httpClient.PostAsJsonAsync(Path.Combine(urlPath, "search"), GetFilter());

                response.EnsureSuccessStatusCode();

                IEnumerable<MunicipalityModel> result = await response.Content.ReadFromJsonAsync<IEnumerable<MunicipalityModel>>();

                return result?.Select(item => new MunicipalityModel
                {
                    Name = item.Name,
                    BfsNumber = item.BfsNumber,
                    Canton = item.Canton,
                    MutationId = item.MutationId,
                    DateOfMutation = item.DateOfMutation,
                    SuccessorId = item.SuccessorId
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            MunicipalitySearchFilter GetFilter()
            {
                return new MunicipalitySearchFilter
                {
                    YearOfValidity = 2021
                };
            }
        }
    }
}
