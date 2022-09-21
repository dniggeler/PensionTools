using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;

namespace BlazorApp.Services;

public class MockCapitalBenefitsComparisonService : ITaxCapitalBenefitsComparisonService
{
    public async IAsyncEnumerable<CapitalBenefitTaxComparerResponse> CalculateAsync(CapitalBenefitTaxComparerRequest request)
    {
        await Task.Delay(500);

        yield return new CapitalBenefitTaxComparerResponse
        {
            MunicipalityName = "Zürich",
            MunicipalityId = 261,
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
