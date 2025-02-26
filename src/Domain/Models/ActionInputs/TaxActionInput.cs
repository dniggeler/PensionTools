namespace Domain.Models.ActionInputs;

public class TaxActionInput
{
    public Guid TaxPaymentSourceAccountId { get; set; }

    public Guid TaxPaymentTargetAccountId { get; set; }

    public TaxBalanceAction[] BalanceActions { get; set; } = [];
}
