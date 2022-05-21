using PensionCoach.Tools.EstvTaxCalculators.Models;

namespace PensionCoach.Tools.EstvTaxCalculators;

public interface IEstvTaxCalculatorClient
{
    Task<TaxLocation> GetTaxLocationAsync(string zip, string city);
}
