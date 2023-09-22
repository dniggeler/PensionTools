using Application.Features.FullTaxCalculation;
using Application.Tax.Estv.Client;
using Application.Tax.Estv.Client.Models;
using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Estv;

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
