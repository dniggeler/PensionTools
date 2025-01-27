using Calculator;

namespace MultiPeriodGraphClient;

public interface IBalanceSheetGenerator
{
    BalanceSheet GenerateBalanceSheet(DateTime startDate, DateTime endDate, ICalculate_Calculate_Transactions calculateTransactions);
}
