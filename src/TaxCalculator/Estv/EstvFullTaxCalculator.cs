using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Estv;

public class EstvFullTaxCalculator : IFullWealthAndIncomeTaxCalculator
{
    private readonly IEstvTaxCalculatorClient estvTaxCalculatorClient;
    private readonly ITaxSupportedYearProvider taxSupportedYearProvider;

    public EstvFullTaxCalculator(IEstvTaxCalculatorClient estvTaxCalculatorClient, ITaxSupportedYearProvider taxSupportedYearProvider)
    {
        this.estvTaxCalculatorClient = estvTaxCalculatorClient;
        this.taxSupportedYearProvider = taxSupportedYearProvider;
    }

    public async Task<Either<string, FullTaxResult>> CalculateAsync(
        int calculationYear, MunicipalityModel municipality, TaxPerson person, bool withMaxAvailableCalculationYear = false)
    {
        if (!municipality.EstvTaxLocationId.HasValue)
        {
            return "ESTV tax location id is null";
        }

        SimpleTaxResult estvResult = await estvTaxCalculatorClient.CalculateIncomeAndWealthTaxAsync(
            municipality.EstvTaxLocationId.Value, calculationYear, person);

        decimal simpleTaxRate = decimal.Zero;
        if (estvResult.IncomeTaxCanton > decimal.Zero)
        {
            simpleTaxRate = estvResult.IncomeTaxCity / (decimal)estvResult.IncomeTaxCanton * 100M;
        }

        decimal wealthTaxRate = decimal.Zero;
        if (estvResult.FortuneTaxCanton > decimal.Zero)
        {
            simpleTaxRate = estvResult.FortuneTaxCity / (decimal)estvResult.FortuneTaxCanton * 100M;
        }

        return new FullTaxResult
        {
            FederalTaxResult = new BasisTaxResult
            {
                TaxAmount = estvResult.IncomeTaxFed,
                DeterminingFactorTaxableAmount = decimal.Zero
            },
            StateTaxResult = new StateTaxResult
            {
                BasisIncomeTax = new BasisTaxResult
                {
                    TaxAmount = estvResult.IncomeTaxCanton,
                    DeterminingFactorTaxableAmount = simpleTaxRate
                },
                BasisWealthTax = new BasisTaxResult
                {
                    TaxAmount = estvResult.FortuneTaxCanton,
                    DeterminingFactorTaxableAmount = wealthTaxRate
                },
                ChurchTax = new ChurchTaxResult
                {
                    TaxAmount = estvResult.IncomeTaxChurch + estvResult.FortuneTaxChurch,
                    TaxAmountPartner = Option<decimal>.None,
                },
                PollTaxAmount = estvResult.PersonalTax,
                CantonRate = 100M,
                MunicipalityRate = simpleTaxRate
            }
        };
    }
}
