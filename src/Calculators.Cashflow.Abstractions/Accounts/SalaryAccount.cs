using System;
using System.Collections.Generic;

namespace Calculators.CashFlow.Accounts;

public class SalaryAccount
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Balance { get; set; }

    public IEnumerable<AccountTransaction> Transactions { get; set; }
}
