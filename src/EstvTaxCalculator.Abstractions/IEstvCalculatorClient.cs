using Domain.Models.Tax;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;

namespace PensionCoach.Tools.EstvTaxCalculators.Abstractions;

public interface IEstvTaxCalculatorClient
{
    Task<TaxLocation[]> GetTaxLocationsAsync(string zip, string city);

    Task<SimpleTaxResult> CalculateIncomeAndWealthTaxAsync(int taxLocationId, int taxYear, TaxPerson person);

    Task<SimpleCapitalTaxResult> CalculateCapitalBenefitTaxAsync(int taxLocationId, int taxYear, CapitalBenefitTaxPerson person);
}
