using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Municipality;
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

    public EstvFullCapitalBenefitTaxCalculator(
        IEstvTaxCalculatorClient estvTaxCalculatorClient)
    {
        this.estvTaxCalculatorClient = estvTaxCalculatorClient;
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

        SimpleCapitalTaxResult calculationResult = await estvTaxCalculatorClient.CalculateCapitalBenefitTaxAsync(
            municipality.EstvTaxLocationId.Value, calculationYear, person);

        decimal municipalityRate = calculationResult.TaxCity / (decimal)calculationResult.TaxCanton * 100M;

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
