namespace Domain.Models.AccountInputs;

public abstract class AccountInputBase
{
    public Guid Id { get; set; }

    public string? Description { get; set; }
}
