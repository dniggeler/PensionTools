using System;
using System.Collections.Generic;

namespace Calculators.CashFlow.Accounts;

public class TaxAccount : ICashFlowAccount
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Balance { get; set; }

    public decimal NetGrowthRate { get; set; }

    public List<AccountTransaction> Transactions { get; set; } = new();
}
