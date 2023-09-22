using Application.Tax.Estv.Client.Models;
using Domain.Models.Tax;

namespace Application.Tax.Estv.Client
{
    public interface IEstvTaxCalculatorClient
    {
        Task<TaxLocation[]> GetTaxLocationsAsync(string zip, string city);

        Task<SimpleTaxResult> CalculateIncomeAndWealthTaxAsync(int taxLocationId, int taxYear, TaxPerson person);

        Task<SimpleCapitalTaxResult> CalculateCapitalBenefitTaxAsync(int taxLocationId, int taxYear, CapitalBenefitTaxPerson person);
    }
}
