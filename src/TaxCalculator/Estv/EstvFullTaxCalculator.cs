using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Estv;

public class EstvFullTaxCalculator : IFullTaxCalculator
{
    private readonly IEstvTaxCalculatorClient estvTaxCalculatorClient;

    public EstvFullTaxCalculator(IEstvTaxCalculatorClient estvTaxCalculatorClient)
    {
        this.estvTaxCalculatorClient = estvTaxCalculatorClient;
    }

    public async Task<Either<string, FullTaxResult>> CalculateAsync(
        int calculationYear, int taxId, Canton canton, TaxPerson person, bool withMaxAvailableCalculationYear = false)
    {
        SimpleTaxResult result = await estvTaxCalculatorClient.CalculateIncomeAndWealthTaxAsync(
            calculationYear, taxId, person);

        decimal municipalityRate = result.IncomeTaxCity / (decimal)result.IncomeSimpleTaxCity * 100M;

        Either<string, FullTaxResult> fullTaxResult = new FullTaxResult
        {
            FederalTaxResult = new BasisTaxResult
            {
                TaxAmount = result.IncomeTaxFed,
                DeterminingFactorTaxableAmount = decimal.Zero
            },
            StateTaxResult = new StateTaxResult
            {
                BasisIncomeTax = new BasisTaxResult
                {
                    TaxAmount = result.IncomeSimpleTaxCity,
                    DeterminingFactorTaxableAmount = municipalityRate
                },
                BasisWealthTax = new BasisTaxResult
                {
                    TaxAmount = result.FortuneSimpleTaxCanton,
                    DeterminingFactorTaxableAmount = result.FortuneTaxCity / (decimal)result.FortuneSimpleTaxCity
                },
                ChurchTax = new ChurchTaxResult
                {
                    TaxAmount = result.IncomeTaxChurch + result.FortuneTaxChurch,
                    TaxAmountPartner = Option<decimal>.None,
                },
                PollTaxAmount = result.PersonalTax,
                CantonRate = 100M,
                MunicipalityRate = municipalityRate
            }
        };

        return fullTaxResult;
    }
}
