using System.Collections.Generic;
using Calculators.CashFlow.Models;

namespace TaxCalculator.WebApi.Models
{
    public class MultiPeriodResponse
    {
        public int StartingYear { get; set; }

        public int NumberOfPeriods { get; set; }

        public IEnumerable<SinglePeriodCalculationResult> Accounts { get; set; }
    }
}
