using Domain.Models.Cashflows.Accounts;

namespace Domain.Models.Cashflows
{
    public record AccountTransactionResult
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<AccountTransaction> Transactions { get; set; } = new();
    }
}
