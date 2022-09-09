using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;

namespace PensionCoach.Tools.EstvTaxCalculators;

public class EstvTaxCalculatorClientMock : IEstvTaxCalculatorClient
{
    public EstvTaxCalculatorClientMock(Proprietary)
    {
        
    }

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
        throw new NotImplementedException();
    }

    public Task<SimpleCapitalTaxResult> CalculateCapitalBenefitTaxAsync(int taxLocationId, int taxYear, CapitalBenefitTaxPerson person)
    {
        throw new NotImplementedException();
    }
}
