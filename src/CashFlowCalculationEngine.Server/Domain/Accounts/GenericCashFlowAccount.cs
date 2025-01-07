namespace CashFlowCalculationEngine.Server.Domain.Accounts;

[InterfaceType]
public abstract class GenericCashFlowAccount
{
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public List<AccountTransaction>? Transactions { get; set; } = [];
}
