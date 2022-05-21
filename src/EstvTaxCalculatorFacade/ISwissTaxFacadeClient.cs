using PensionCoach.Tools.EstvTaxCalculators.Models;

namespace PensionCoach.Tools.EstvTaxCalculators;

public interface ISwissTaxFacadeClient
{
    Task<TaxLocation> GetTaxLocationAsync(string zip, string city);
}
