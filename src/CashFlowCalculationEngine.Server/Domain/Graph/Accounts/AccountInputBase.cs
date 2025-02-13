namespace CashFlowCalculationEngine.Server.Domain.Graph.Accounts;

[InterfaceType]
public abstract class AccountInputBase
{
    [GraphQLDescription("Unique account id")]
    public Guid Id { get; set; }

    [GraphQLDescription("Account description")]
    public string? Description { get; set; }
}
