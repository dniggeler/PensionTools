using System;
using System.Collections.Generic;

namespace Calculators.CashFlow.Accounts;

public interface ICashFlowAccount
{
    Guid Id { get; set; }

    string Name { get; set; }

    decimal Balance { get; set; }

    decimal NetGrowthRate { get; set; }

    List<AccountTransaction> Transactions { get; set; }
}
