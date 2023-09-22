namespace Domain.Models.Cashflows.Accounts
{
    public class ThirdPillarAccount : ICashFlowAccount
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public decimal NetGrowthRate { get; set; }

        public List<AccountTransaction> Transactions { get; set; } = new();
    }
}
