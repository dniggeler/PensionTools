using Domain.Enums;

namespace Domain.Models.Cashflows.Accounts
{
    public interface ICashFlowAccount
    {
        Guid Id { get; set; }

        string Name { get; set; }

        decimal Balance { get; set; }

        decimal NetGrowthRate { get; set; }

        AccountType AccountType { get; }

        List<AccountTransaction> Transactions { get; set; }
    }
}
