using System.Collections.Generic;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod
{
    public class CapitalBenefitsTransferInResponse
    {
        public int StartingYear { get; set; }

        public int NumberOfPeriods { get; set; }

        public IEnumerable<SinglePeriodCalculationResult> Accounts { get; set; }
    }
}
