using System.Collections.Generic;
using Domain.Models.MultiPeriod;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod
{
    public class MultiPeriodResponse
    {
        public int StartingYear { get; set; }

        public int NumberOfPeriods { get; set; }

        public IEnumerable<SinglePeriodCalculationResult> Accounts { get; set; }
    }
}
