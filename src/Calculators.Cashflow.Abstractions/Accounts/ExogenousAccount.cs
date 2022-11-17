using System;
using System.Collections.Generic;

namespace Calculators.CashFlow.Accounts;

public class ExogenousAccount : ICashFlowAccount
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Balance { get; set; }

    public decimal InterestRate { get; set; }

    public List<AccountTransaction> Transactions { get; set; } = new();
}
