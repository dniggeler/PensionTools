using Domain.Models.Cashflows;
using Domain.Models.MultiPeriod;

namespace Domain.Models.Scenarios;

public record ScenarioCalculationResult
{
    public int StartingYear { get; set; }

    public int NumberOfPeriods { get; set; }

    public IEnumerable<SinglePeriodCalculationResult> DeltaSeries { get; set; }

    public IEnumerable<SinglePeriodCalculationResult> BenchmarkSeries { get; set; }

    public IEnumerable<SinglePeriodCalculationResult> ScenarioSeries { get; set; }

    public AccountTransactionResultHolder BenchmarkTransactions { get; set; }

    public AccountTransactionResultHolder ScenarioTransactions { get; set; }
}
