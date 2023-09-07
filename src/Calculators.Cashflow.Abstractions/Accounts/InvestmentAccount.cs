using System;
using System.Collections.Generic;

namespace Calculators.CashFlow.Accounts;

/// <summary>
/// Represents an investment account. it models the cash flow of an investment account.
/// However, there are two different types of cash flows:
/// 1. the cash flow of the capital gains
/// 2. the cash flow of the dividends or interests
/// </summary>
public class InvestmentAccount : ICashFlowAccount
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Balance { get; set; }

    /// <summary>
    /// Gets or sets the net income yield. Income types are dividends and interests which are subject to income tax.
    /// This is the yield before income taxes.
    /// </summary>
    public decimal NetIncomeYield { get; set; }

    /// <summary>
    /// Gets or sets the net rate of return. This is the yield before wealth taxes,
    /// and does not include the dividends.
    /// </summary>
    public decimal NetGrowthRate { get; set; }

    public List<AccountTransaction> Transactions { get; set; } = new();
}
