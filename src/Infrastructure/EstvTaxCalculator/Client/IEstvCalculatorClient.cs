using Domain.Models.Tax;
using Infrastructure.EstvTaxCalculator.Client.Models;

namespace Infrastructure.EstvTaxCalculator.Client;

public interface IEstvTaxCalculatorClient
{
    Task<TaxLocation[]> GetTaxLocationsAsync(string zip, string city);

    Task<SimpleTaxResult> CalculateIncomeAndWealthTaxAsync(int taxLocationId, int taxYear, TaxPerson person);

    Task<SimpleCapitalTaxResult> CalculateCapitalBenefitTaxAsync(int taxLocationId, int taxYear, CapitalBenefitTaxPerson person);
}
