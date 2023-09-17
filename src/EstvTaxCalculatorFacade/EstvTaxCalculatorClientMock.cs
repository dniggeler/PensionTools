using Domain.Models.Tax;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;

namespace PensionCoach.Tools.EstvTaxCalculators;

public class EstvTaxCalculatorClientMock : IEstvTaxCalculatorClient
{

    public Task<TaxLocation[]> GetTaxLocationsAsync(string zip, string city)
    {
        return Task.FromResult(new[]
        {
            new TaxLocation
            {
                BfsId = 261,
                BfsName = "Zürich",
                Canton = "ZH",
                CantonId = 1,
                City = "Zürich",
                ZipCode = "8000"
            }
        });
    }

    public Task<SimpleTaxResult> CalculateIncomeAndWealthTaxAsync(int taxLocationId, int taxYear, TaxPerson person)
    {
        return Task.FromResult(new SimpleTaxResult
        {
            IncomeSimpleTaxCanton = 6296,
            FortuneTaxCanton = 308,
            IncomeSimpleTaxCity = 6296,
            IncomeTaxChurch = 630,
            IncomeTaxCity = 7492,
            IncomeSimpleTaxFed = 2874,
            PersonalTax = 24,
            FortuneTaxCity = 366,
            FortuneSimpleTaxCanton = 308,
            IncomeTaxFed = 2874,
            FortuneSimpleTaxCity = 308,
            IncomeTaxCanton = 6296,
            FortuneTaxChurch = 31,
            Location =
            {
                Id = 80000000,
                ZipCode = "800",
                BfsId = 261,
                CantonId = 26,
                BfsName = "Züich",
                City = "Zürich",
                Canton = "ZH"
            }
        });
    }

    public Task<SimpleCapitalTaxResult> CalculateCapitalBenefitTaxAsync(int taxLocationId, int taxYear, CapitalBenefitTaxPerson person)
    {
        return Task.FromResult(new SimpleCapitalTaxResult
        {
            TaxCanton = 20870,
            TaxChurch = 2087,
            TaxCity = 24835,
            TaxFed = 10632,
            Location =
            {
                Id = 800000000,
                ZipCode = "8000",
                BfsId = 261,
                CantonId = 26,
                BfsName = "Zürich",
                City = "Zürich",
                Canton = "ZH"
            }
        });
    }
}
