using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Tax.Estv.Client;
using Application.Tax.Estv.Client.Models;
using Domain.Enums;
using Domain.Models.Tax;
using Infrastructure.EstvTaxCalculator.Models;
using Microsoft.Extensions.Logging;
using Replicant;

namespace Infrastructure.EstvTaxCalculator
{
    public class EstvTaxCalculatorClient(IHttpClientFactory httpClientFactory, ILogger<EstvTaxCalculatorClient> logger)
        : IEstvTaxCalculatorClient
    {
        internal static string EstvTaxCalculatorClientName = "EstvTaxCalculatorClient";

        public async Task<TaxLocation[]> GetTaxLocationsAsync(string zip, string city)
        {
            HttpClient client = httpClientFactory.CreateClient("EstvTaxCalculatorClient");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            var request = new TaxLocationRequest { Search = string.IsNullOrEmpty(city) ? $"{zip}" : $"{zip} {city}" };

            TaxLocationResponse response = await CallAsync<TaxLocationResponse>(JsonSerializer.Serialize(request), "API_searchLocation");

            return response.Response;
        }

        public async Task<SimpleTaxResult> CalculateIncomeAndWealthTaxAsync(int taxLocationId, int taxYear, TaxPerson person)
        {
            SimpleTaxRequest request = new SimpleTaxRequest
            {
                TaxYear = taxYear,
                TaxLocationId = taxLocationId,
                Children = Array.Empty<ChildModel>(),
                Confession1 = Map(person.ReligiousGroupType),
                Confession2 = 0,
                Relationship = MapCivilStatus(person.CivilStatus),
                TaxableIncomeFed = (int)person.TaxableFederalIncome,
                TaxableFortune = (int)person.TaxableWealth,
                TaxableIncomeCanton = (int)person.TaxableIncome
            };

            if (person.CivilStatus == CivilStatus.Married)
            {
                request.Confession2 = Map(person.PartnerReligiousGroupType);
            }

            SimpleTaxResponse response = await CallAsync<SimpleTaxResponse>(JsonSerializer.Serialize(request), "API_calculateSimpleTaxes");

            return response.Response;    
        }

        public async Task<SimpleCapitalTaxResult> CalculateCapitalBenefitTaxAsync(int taxLocationId, int taxYear, CapitalBenefitTaxPerson person)
        {
            SimpleCapitalTaxRequest request = new()
            {
                TaxYear = taxYear,
                TaxGroupId = taxLocationId,
                AgeAtRetirement = 65,
                NumberOfChildren = 0,
                Confession1 = Map(person.ReligiousGroupType),
                Confession2 = 0,
                Relationship = MapCivilStatus(person.CivilStatus),
                Capital = (int)person.TaxableCapitalBenefits,
            };

            if (person.CivilStatus == CivilStatus.Married)
            {
                request.Confession2 = Map(person.PartnerReligiousGroupType);
            }

            SimpleCapitalTaxResponse response =
                await CallAsync<SimpleCapitalTaxResponse>(JsonSerializer.Serialize(request), "API_calculateManyCapitalTaxes");

            return response.Response.FirstOrDefault();
        }

        private async Task<TOut> CallAsync<TOut>(string request, string path)
        {
            HttpClient client = httpClientFactory.CreateClient("EstvTaxCalculatorClient");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            HttpCache cache = new HttpCache("/temp/replicant", client, 10000);

            string hashRequest = Hash.Compute(request);

            string realUri = $"https://swisstaxcalculator.estv.admin.ch/delegate/ost-integration/v1/lg-proxy/operation/c3b67379_ESTV/{path}";
            string cacheUri = $"{realUri}?tag={hashRequest}";
            bool cacheHit = true;
            HttpResponseMessage response = await cache.ResponseAsync(
                cacheUri, true, m =>
                {
                    cacheHit = false;
                    m.Method = HttpMethod.Post;
                    m.RequestUri = new Uri(realUri);
                    m.Content = content;
                }, CancellationToken.None);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            if (cacheHit is false)
            {
                await cache.AddItemAsync(cacheUri, json, null, null, null, null, null, null, CancellationToken.None);
            }
            else
            {
                logger.LogInformation($"Cache hit for ESTV calculator");
            }

            return JsonSerializer.Deserialize<TOut>(json);
        }

        private static int MapCivilStatus(CivilStatus civilStatus)
        {
            return civilStatus switch
            {
                CivilStatus.Single => 1,
                CivilStatus.Married => 2,
                _ => 2
            };
        }

        private static int Map(ReligiousGroupType? religiousGroupType)
        {
            return religiousGroupType switch
            {
                null => 0,
                ReligiousGroupType.Protestant => 1,
                ReligiousGroupType.Roman => 2,
                ReligiousGroupType.Catholic => 3,
                ReligiousGroupType.Other => 5,
                _ => 5
            };
        }
    }
}
