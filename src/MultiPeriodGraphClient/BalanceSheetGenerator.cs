using Calculator;

namespace MultiPeriodGraphClient;

public class BalanceSheetGenerator : IBalanceSheetGenerator
{
    public BalanceSheet GenerateBalanceSheet(DateTime startDate, DateTime endDate, ICalculate_Calculate_Transactions calculateTransactions)
    {
        BalanceSheet.BalanceSheetEntry entry = new BalanceSheet.BalanceSheetEntry();

        entry.TotalWealth =
            calculateTransactions.IncomeAccounts.SelectMany(a => a.Transactions ?? [])
                .Sum(t => t.Amount) +
            calculateTransactions.WealthAccounts.SelectMany(a => a.Transactions ?? [])
                .Sum(t => t.Amount);

        return new BalanceSheet
        {
            Entries = [ entry ]
        };
    }
}
