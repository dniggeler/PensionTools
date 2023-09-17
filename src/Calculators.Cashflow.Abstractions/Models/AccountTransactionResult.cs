using System;
using System.Collections.Generic;
using Calculators.CashFlow.Accounts;

namespace Calculators.CashFlow.Models;

public record AccountTransactionResult
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public List<AccountTransaction> Transactions { get; set; } = new();
}
