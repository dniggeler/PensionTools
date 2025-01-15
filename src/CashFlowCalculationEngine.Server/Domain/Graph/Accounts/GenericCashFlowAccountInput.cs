using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.Graph.Accounts;

[InterfaceType]
public abstract class GenericCashFlowAccountInput
{
    [GraphQLDescription("Unique account id")]
    public Guid Id { get; set; }

    [GraphQLDescription("Account description")]
    public string? Description { get; set; }
}
