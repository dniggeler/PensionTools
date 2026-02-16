using Application.Features.FullTaxCalculation;
using Application.Tax.Contracts;
using Application.Tax.Estv.Client;
using Application.Tax.Estv.Client.Models;
using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Estv;

/// <summary>
/// Facade for the ESTV tax calculation service.
/// </summary>
public class EstvFullCapitalBenefitTaxCalculator(
    IEstvTaxCalculatorClient estvTaxCalculatorClient,
    ITaxSupportedYearProvider taxSupportedYearProvider)
    : IFullCapitalBenefitTaxCalculator
{
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

        return await CalculateAsync(calculationYear, municipality.EstvTaxLocationId.Value, person);
    }

    public async Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(int calculationYear, long taxLocationId, CapitalBenefitTaxPerson person)
    {
        int supportedTaxYear = taxSupportedYearProvider.MapToSupportedYear(calculationYear);

        SimpleCapitalTaxResult calculationResult =
            await estvTaxCalculatorClient.CalculateCapitalBenefitTaxAsync((int)taxLocationId, supportedTaxYear, person);

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
