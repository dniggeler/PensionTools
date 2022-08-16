using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator.Estv;

public class EstvFullTaxCalculator : IFullTaxCalculator
{
    public Task<Either<string, FullTaxResult>> CalculateAsync(
        int calculationYear, int municipalityId, Canton canton, TaxPerson person, bool withMaxAvailableCalculationYear = false)
    {
        Either<string, FullTaxResult> fullTaxResult = new FullTaxResult
        {
            FederalTaxResult = new BasisTaxResult
            {
                TaxAmount = decimal.Zero, DeterminingFactorTaxableAmount = decimal.Zero
            },
            StateTaxResult = new StateTaxResult
            {
                BasisIncomeTax = new BasisTaxResult
                {
                    TaxAmount = decimal.Zero
                },
                BasisWealthTax = new BasisTaxResult
                {
                    TaxAmount = decimal.Zero
                },
                ChurchTax = new ChurchTaxResult
                {
                    TaxAmount = decimal.Zero,
                    TaxAmountPartner = Option<decimal>.None,
                },
                PollTaxAmount = decimal.Zero,
                CantonRate = decimal.Zero,
                MunicipalityRate = decimal.Zero
            }
        };

        return fullTaxResult.AsTask();
    }
}
