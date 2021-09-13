using System.Collections.Generic;

namespace Calculators.CashFlow.Models
{
    public record MultiPeriodCalculationResult
    {
        public int StartingYear { get; set; }
        
        public int NumberOfPeriods { get; set; }

        public IEnumerable<SinglePeriodCalculationResult> Accounts{ get; set; }
    }
}
