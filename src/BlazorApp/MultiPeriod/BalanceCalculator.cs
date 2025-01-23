using System;
using System.Linq;
using Calculator;

namespace BlazorApp.MultiPeriod;

public class BalanceCalculator
{
    public BalanceSheet Calculate(DateTime startDate, DateTime endDate, ICalculate_Calculate_Transactions calculateTransactions)
    {
        BalanceSheet balanceSheet = new BalanceSheet();

        balanceSheet.TotalWealth =
            calculateTransactions.IncomeAccounts.SelectMany(a => a.Transactions)
                .Sum(t => t.Amount) +
            calculateTransactions.WealthAccounts.SelectMany(a => a.Transactions)
                .Sum(t => t.Amount);

        return balanceSheet;
    }
}
