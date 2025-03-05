using Application.MultiPeriodCalculator.ExcelManager;
using Calculator;
using Domain.Enums;
using FlowType = Domain.Enums.FlowType;

namespace MultiPeriodCalculator.Console;

internal static class ConsoleHelperExtensions
{
    public static IEnumerable<ExcelResponseTransaction> CreateExcelTransactions(
        this IReadOnlyList<ICalculate_Calculate_Transactions_IncomeAccounts> accounts)
    {
        foreach (var account in accounts)
        {
            foreach (var trx in account.Transactions ?? [])
            {
                if (trx is not null)
                {
                    yield return new ExcelResponseTransaction(
                        account.Id,
                        AccountType.Income,
                        account.Name,
                        trx.Description,
                        trx.Amount,
                        new DateOnly(trx.ValutaDate.Year, trx.ValutaDate.Month, trx.ValutaDate.Day),
                        (FlowType)(int)trx.Flow);
                }
            }
        }
    }

    public static IEnumerable<ExcelResponseTransaction> CreateExcelTransactions(
        this IReadOnlyList<ICalculate_Calculate_Transactions_WealthAccounts> accounts)
    {
        foreach (var account in accounts)
        {
            foreach (var trx in account.Transactions ?? [])
            {
                if (trx is not null)
                {
                    yield return new ExcelResponseTransaction(
                        account.Id,
                        AccountType.Wealth,
                        account.Name,
                        trx.Description,
                        trx.Amount,
                        new DateOnly(trx.ValutaDate.Year, trx.ValutaDate.Month, trx.ValutaDate.Day),
                        (FlowType)(int)trx.Flow);
                }
            }
        }
    }

    public static IEnumerable<ExcelResponseTransaction> CreateExcelTransactions(
        this IReadOnlyList<ICalculate_Calculate_Transactions_ExogenousAccounts> accounts)
    {
        foreach (var account in accounts)
        {
            foreach (var trx in account.Transactions ?? [])
            {
                if (trx is not null)
                {
                    yield return new ExcelResponseTransaction(
                        account.Id,
                        AccountType.Exogenous,
                        account.Name,
                        trx.Description,
                        trx.Amount,
                        new DateOnly(trx.ValutaDate.Year, trx.ValutaDate.Month, trx.ValutaDate.Day),
                        (FlowType)(int)trx.Flow);
                }
            }
        }
    }

    public static IEnumerable<ExcelResponseTransaction> CreateExcelTransactions(
        this IReadOnlyList<ICalculate_Calculate_Transactions_OccupationalPensionAccounts> accounts)
    {
        foreach (var account in accounts)
        {
            foreach (var trx in account.Transactions ?? [])
            {
                if (trx is not null)
                {
                    yield return new ExcelResponseTransaction(
                        account.Id,
                        AccountType.OccupationalPension,
                        account.Name,
                        trx.Description,
                        trx.Amount,
                        new DateOnly(trx.ValutaDate.Year, trx.ValutaDate.Month, trx.ValutaDate.Day),
                        (FlowType)(int)trx.Flow);
                }
            }
        }
    }

    public static IEnumerable<ExcelResponseTransaction> CreateExcelTransactions(
        this IReadOnlyList<ICalculate_Calculate_Transactions_ThirdPillarAccounts> accounts)
    {
        foreach (var account in accounts)
        {
            foreach (var trx in account.Transactions ?? [])
            {
                if (trx is not null)
                {
                    yield return new ExcelResponseTransaction(
                        account.Id,
                        AccountType.ThirdPillar,
                        account.Name,
                        trx.Description,
                        trx.Amount,
                        new DateOnly(trx.ValutaDate.Year, trx.ValutaDate.Month, trx.ValutaDate.Day),
                        (FlowType)(int)trx.Flow);
                }
            }
        }
    }
}
