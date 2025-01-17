using Domain.Enums;
using Domain.Models.Cashflows.Accounts;

namespace CashFlowCalculationEngine.Server.Domain.Accounts;

public class InternalTaxAccount
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }

    public TaxType TaxType { get; set; }

    public decimal Balance { get; set; }

    public List<AccountTransaction> Transactions { get; set; } = [];
}
