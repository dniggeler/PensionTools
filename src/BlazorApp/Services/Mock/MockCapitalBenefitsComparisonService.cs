using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;

namespace BlazorApp.Services.Mock;

public class MockCapitalBenefitsComparisonService : ITaxCapitalBenefitsComparisonService
{
    public async IAsyncEnumerable<CapitalBenefitTaxComparerResponse> CalculateAsync(CapitalBenefitTaxComparerRequest request)
    {
        await Task.Delay(500);

        yield return new CapitalBenefitTaxComparerResponse
        {
            MunicipalityName = "Bagnes",
            MunicipalityId = 6031,
            Canton = Canton.VS,
            MaxSupportedTaxYear = 2022,
            Name = "Mock 1",
            TotalTaxAmount = 121_000,
            TaxDetails = new TaxAmountDetail
            {
                CantonTaxAmount = 60_000,
                FederalTaxAmount = 15_000,
                MunicipalityTaxAmount = 45_000,
                ChurchTaxAmount = 1000
            }
        };

        await Task.Delay(500);

        yield return new CapitalBenefitTaxComparerResponse
        {
            MunicipalityName = "Bern",
            MunicipalityId = 351,
            Canton = Canton.BE,
            MaxSupportedTaxYear = 2022,
            Name = "Mock 2",
            TotalTaxAmount = 131_000,
            TaxDetails = new TaxAmountDetail
            {
                CantonTaxAmount = 50_000,
                FederalTaxAmount = 15_000,
                MunicipalityTaxAmount = 65_000,
                ChurchTaxAmount = 1000
            }
        };

        await Task.Delay(500);

        yield return new CapitalBenefitTaxComparerResponse
        {
            MunicipalityName = "Zürich",
            MunicipalityId = 261,
            Canton = Canton.ZH,
            MaxSupportedTaxYear = 2022,
            Name = "Mock 1",
            TotalTaxAmount = 101_000,
            TaxDetails = new TaxAmountDetail
            {
                CantonTaxAmount = 40_000,
                FederalTaxAmount = 15_000,
                MunicipalityTaxAmount = 45_000,
                ChurchTaxAmount = 1000
            }
        };

        await Task.Delay(500);

        yield return new CapitalBenefitTaxComparerResponse
        {
            MunicipalityName = "Lachen",
            MunicipalityId = 1344,
            Canton = Canton.SZ,
            MaxSupportedTaxYear = 2022,
            Name = "Mock 2",
            TotalTaxAmount = 81_000,
            TaxDetails = new TaxAmountDetail
            {
                CantonTaxAmount = 30_000,
                FederalTaxAmount = 15_000,
                MunicipalityTaxAmount = 35_000,
                ChurchTaxAmount = 1000
            }
        };
    }
}
