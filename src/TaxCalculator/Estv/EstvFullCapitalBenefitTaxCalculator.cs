using System.Threading.Tasks;
using Domain.Models.Municipality;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Estv;

/// <summary>
/// Facade for the ESTV tax calculation service.
/// </summary>
public class EstvFullCapitalBenefitTaxCalculator : IFullCapitalBenefitTaxCalculator
{
    private readonly IEstvTaxCalculatorClient estvTaxCalculatorClient;
    private readonly ITaxSupportedYearProvider taxSupportedYearProvider;

    public EstvFullCapitalBenefitTaxCalculator(
        IEstvTaxCalculatorClient estvTaxCalculatorClient,
        ITaxSupportedYearProvider taxSupportedYearProvider)
    {
        this.estvTaxCalculatorClient = estvTaxCalculatorClient;
        this.taxSupportedYearProvider = taxSupportedYearProvider;
    }

    public async Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear,
        MunicipalityModel municipality,
        CapitalBenefitTaxPerson person,
        bool withMaxAvailableCalculationYear = false)
    {
        if (!municipality.EstvTaxLocationId.HasValue)
        {
            return "ESTV tax location id is null";
        }

        int supportedTaxYear = taxSupportedYearProvider.MapToSupportedYear(calculationYear);

        SimpleCapitalTaxResult calculationResult = await estvTaxCalculatorClient.CalculateCapitalBenefitTaxAsync(
            municipality.EstvTaxLocationId.Value, supportedTaxYear, person);

        decimal municipalityRate = calculationResult.TaxCanton == 0
            ? decimal.Zero
            : calculationResult.TaxCity / (decimal)calculationResult.TaxCanton * 100M;

        return new FullCapitalBenefitTaxResult
        {
            FederalResult = new BasisTaxResult { TaxAmount = calculationResult.TaxFed },
            StateResult = new CapitalBenefitTaxResult
            {
                MunicipalityRate = municipalityRate,
                CantonRate = 100,
                ChurchTax = new ChurchTaxResult
                {
                    TaxAmount = calculationResult.TaxChurch,
                },
                BasisTax = new BasisTaxResult
                {
                    TaxAmount = calculationResult.TaxCanton,
                    DeterminingFactorTaxableAmount = municipalityRate
                }
            }
        };
    }
}
