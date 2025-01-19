using Domain.Enums;

namespace Domain.Models.Cashflows.Accounts
{
    public class WealthAccount : ICashFlowAccount
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public decimal NetGrowthRate { get; set; }

        public AccountType AccountType => AccountType.Wealth;

        public List<AccountTransaction> Transactions { get; set; } = [];
    }
}
