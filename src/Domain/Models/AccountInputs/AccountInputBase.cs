using HotChocolate;
using HotChocolate.Types;

namespace Domain.Models.AccountInputs;

[InterfaceType]
public abstract class AccountInputBase
{
    [GraphQLDescription("Unique account id")]
    public Guid Id { get; set; }

    [GraphQLDescription("Account description")]
    public string? Description { get; set; }
}
