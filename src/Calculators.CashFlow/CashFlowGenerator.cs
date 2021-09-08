using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Abstractions.Models;

namespace Calculators.CashFlow
{
    public class CashFlowGenerator
    {
        IEnumerable<(int year, decimal value)> Generate(GenericCashFlowDefinition definition)
        {
            return Enumerable.Empty<(int year, decimal value)>();
        }
    }
}
