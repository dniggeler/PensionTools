using Application.Municipality;
using Application.Tax.Contracts;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Features.FullTaxCalculation;

public class TaxCalculatorConnector(
    IFullWealthAndIncomeTaxCalculator fullWealthAndIncomeTaxCalculator,
    IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator,
    IMunicipalityConnector municipalityResolver)
    : ITaxCalculatorConnector
{
    private readonly int[] supportedTaxYears = [2019];

    public async Task<Either<string, FullTaxResult>> CalculateAsync(
        int calculationYear, int bfsMunicipalityId, TaxPerson person, bool withMaxAvailableCalculationYear = false)
    {
        Either<string, MunicipalityModel> municipalityData =
            await municipalityResolver.GetAsync(bfsMunicipalityId, calculationYear);

        return await municipalityData
            .BindAsync(m => fullWealthAndIncomeTaxCalculator.CalculateAsync(
                calculationYear, m, person));
    }

    public async Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear, int bfsMunicipalityId, CapitalBenefitTaxPerson person, bool withMaxAvailableCalculationYear = false)
    {
        Either<string, MunicipalityModel> municipalityData =
            await municipalityResolver.GetAsync(bfsMunicipalityId, calculationYear);

        return await municipalityData
            .BindAsync(m => fullCapitalBenefitTaxCalculator.CalculateAsync(
                calculationYear,
                m,
                person));
    }

    public Task<int[]> GetSupportedTaxYears()
    {
        return supportedTaxYears.AsTask();
    }
}
