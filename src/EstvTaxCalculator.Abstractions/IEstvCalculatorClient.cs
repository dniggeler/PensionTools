using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;

namespace PensionCoach.Tools.EstvTaxCalculators.Abstractions;

public interface IEstvTaxCalculatorClient
{
    Task<TaxLocation[]> GetTaxLocationsAsync(string zip, string city);

    Task<SimpleTaxResult> CalculateIncomeAndWealthTaxAsync(SimpleTaxRequest request);
}
