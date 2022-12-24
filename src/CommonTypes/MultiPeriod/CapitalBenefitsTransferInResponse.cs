using System.Collections.Generic;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod
{
    public class CapitalBenefitsTransferInResponse
    {
        public int StartingYear { get; set; }

        public int NumberOfPeriods { get; set; }

        public IEnumerable<SinglePeriodCalculationResult> DeltaSeries { get; set; }

        public IEnumerable<SinglePeriodCalculationResult> BenchmarkSeries { get; set; }

        public IEnumerable<SinglePeriodCalculationResult> ScenarioSeries { get; set; }
    }
}
