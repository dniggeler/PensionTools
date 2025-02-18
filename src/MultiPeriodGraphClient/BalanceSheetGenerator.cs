using Calculator;

namespace MultiPeriodGraphClient;

public class BalanceSheetGenerator : IBalanceSheetGenerator
{
    public BalanceSheet GenerateBalanceSheet(DateTime startDate, DateTime endDate, ICalculate_Calculate_Transactions? calculateTransactions)
    {
        ArgumentNullException.ThrowIfNull(calculateTransactions);

        BalanceSheet.BalanceSheetEntry entry = new BalanceSheet.BalanceSheetEntry();

        entry.TotalWealth =
            calculateTransactions.IncomeAccounts.SelectMany(a => a.Transactions ?? [])
                .Sum(t => t?.Amount) +
            calculateTransactions.InvestmentAccounts.SelectMany(a => a.Transactions ?? [])
                .Sum(t => t?.Amount) +
            calculateTransactions.WealthAccounts.SelectMany(a => a.Transactions ?? [])
                .Sum(t => t?.Amount);

        entry.TotalThirdPillar =
            calculateTransactions.ThirdPillarAccounts.SelectMany(a => a.Transactions ?? [])
                .Sum(t => t?.Amount);

        entry.TotalOccupationalPension =
            calculateTransactions.OccupationalPensionAccounts.SelectMany(a => a.Transactions ?? [])
                .Sum(t => t?.Amount);

        entry.TotalLiability =
            calculateTransactions.LiabilityAccounts.SelectMany(a => a.Transactions ?? [])
                .Sum(t => t?.Amount);

        return new BalanceSheet
        {
            Entry = entry
        };
    }
}
