using PensionCoach.Tools.EstvTaxCalculators.Models;

namespace PensionCoach.Tools.EstvTaxCalculators;

public interface IEstvFacadeClient
{
    Task<TaxLocation> GetTaxLocationAsync(string zip, string city);
}
