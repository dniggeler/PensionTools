using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Features.FullTaxCalculation;
using Domain.Models.Tax;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services.Mock
{
    public class MockedPensionToolsCalculationService :
        IMultiPeriodCalculationService, ITaxCalculationService, IMarginalTaxCurveCalculationService

    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly ILogger<MockedPensionToolsCalculationService> logger;

        public MockedPensionToolsCalculationService(
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<MockedPensionToolsCalculationService> logger)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<MultiPeriodResponse> CalculateAsync(MultiPeriodRequest request)
        {
            var urlPath = configuration.GetSection("MultiPeriodCalculationServiceUrl").Value;
            foreach (var keyValuePair in configuration.AsEnumerable())
            {
                logger.LogInformation($"{keyValuePair.Key}: {keyValuePair.Value}");
            }

            var response = await httpClient.GetFromJsonAsync<MultiPeriodResponse>(urlPath);

            return response;
        }

        public Task<FullTaxResponse> CalculateAsync(FullTaxRequest request)
        {
            if (request.BfsMunicipalityId == 0)
            {
                throw new ArgumentException(nameof(request.BfsMunicipalityId));
            }

            logger.LogInformation(JsonSerializer.Serialize(request));

            FullTaxResponse taxCalculationResponse = new()
            {
                Name = request.Name,
                CalculationYear = request.CalculationYear,
                CantonTaxAmount = 1000,
                ChurchTaxAmount = 100,
                FederalTaxAmount = 500,
                MunicipalityTaxAmount = 1500,
                PollTaxAmount = 5,
                WealthTaxAmount = 200,
                TotalTaxAmount = 3200,
                TaxRateDetails = new TaxRateDetails
                {
                    CantonRate = 1M,
                    ChurchTaxRate = 0.02M,
                    MunicipalityRate = 1.19M
                }
            };

            return Task.FromResult(taxCalculationResponse);
        }

        public Task<MarginalTaxResponse> CalculateIncomeCurveAsync(MarginalTaxRequest request)
        {
            if (request.BfsMunicipalityId == 0)
            {
                throw new ArgumentException(nameof(request.BfsMunicipalityId));
            }

            var marginalTaxCurve = new List<MarginalTaxInfo>();

            var stepSize = Convert.ToInt32(request.UpperSalaryLimit / 100M);
            var taxRateStepSize = 0.45M / 100.0M;
            var taxAmountStepSize = 20000M / 100.0M;
            for (var ii = 0; ii < 20; ii++)
            {
                for (var jj = 0; jj < 4; jj++)
                {
                    var bucket = ii * 5;
                    marginalTaxCurve.Add(
                        new MarginalTaxInfo((bucket + jj) * stepSize, taxRateStepSize * bucket, taxAmountStepSize * bucket));
                }

                var index = ii * 5 + 4;
                marginalTaxCurve.Add(
                    new MarginalTaxInfo(index * stepSize, taxRateStepSize * index, taxAmountStepSize * index));
            }

            var currentPoint = marginalTaxCurve.Skip(10).Take(1).First();

            MarginalTaxResponse taxCalculationResponse = new()
            {
                CurrentMarginalTaxRate = currentPoint,
                MarginalTaxCurve = marginalTaxCurve
            };

            return Task.FromResult(taxCalculationResponse);
        }

        public Task<int[]> SupportedTaxYearsAsync()
        {
            return new[] { 2019, 2020, 2021, 2022 }.AsTask();
        }
    }
}
