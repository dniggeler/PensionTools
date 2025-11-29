using Calculator;

namespace MultiPeriodGraphClient;

public interface IBalanceSheetGenerator
{
    BalanceSheet GenerateBalanceSheet(DateOnly startDate, DateOnly endDate, ICalculate_Calculate_Transactions? calculateTransactions);
}
