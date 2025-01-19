using Domain.Enums;

namespace Domain.Models.Cashflows.Accounts
{
    public class OccupationalPensionAccount : ICashFlowAccount
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public decimal NetGrowthRate { get; set; }

        public AccountType AccountType => AccountType.OccupationalPension;
        public List<AccountTransaction> Transactions { get; set; } = new();
    }
}
