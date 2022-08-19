using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator.Estv;

public class EstvFullTaxCalculator : IFullTaxCalculator
{
    private readonly IMunicipalityConnector municipalityConnector;
    private readonly IEstvTaxCalculatorClient estvTaxCalculatorClient;

    public EstvFullTaxCalculator(
        IMunicipalityConnector municipalityConnector,
        IEstvTaxCalculatorClient estvTaxCalculatorClient)
    {
        this.municipalityConnector = municipalityConnector;
        this.estvTaxCalculatorClient = estvTaxCalculatorClient;
    }

    public async Task<Either<string, FullTaxResult>> CalculateAsync(
        int calculationYear, int municipalityId, Canton canton, TaxPerson person, bool withMaxAvailableCalculationYear = false)
    {
        Either<string, MunicipalityModel> municipality = await municipalityConnector.GetAsync(municipalityId, calculationYear);

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

        return fullTaxResult;
    }
}
