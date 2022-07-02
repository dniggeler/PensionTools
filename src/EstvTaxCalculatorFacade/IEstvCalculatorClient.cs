using PensionCoach.Tools.EstvTaxCalculators.Models;

namespace PensionCoach.Tools.EstvTaxCalculators;

public interface IEstvTaxCalculatorClient
{
    Task<TaxLocation[]> GetTaxLocationsAsync(string zip, string city);
}
