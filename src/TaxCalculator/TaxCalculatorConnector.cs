using System.Threading.Tasks;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator;

public class TaxCalculatorConnector : ITaxCalculatorConnector
{
    private readonly int[] supportedTaxYears = { 2019 };

    private readonly IFullWealthAndIncomeTaxCalculator fullWealthAndIncomeTaxCalculator;
    private readonly IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator;
    private readonly IMunicipalityConnector municipalityResolver;

    public TaxCalculatorConnector(
        IFullWealthAndIncomeTaxCalculator fullWealthAndIncomeTaxCalculator,
        IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator,
        IMunicipalityConnector municipalityResolver)
    {
        this.fullWealthAndIncomeTaxCalculator = fullWealthAndIncomeTaxCalculator;
        this.fullCapitalBenefitTaxCalculator = fullCapitalBenefitTaxCalculator;
        this.municipalityResolver = municipalityResolver;
    }

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
