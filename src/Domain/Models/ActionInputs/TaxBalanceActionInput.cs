using Domain.Enums;

namespace Domain.Models.ActionInputs;

/// <summary>
/// Represents the input for a tax balance action. All accounts of tax type will be affected by this action.
/// Tax amount is the balance of the account at date of process. Therefore, the source account is derived implicitly.
/// </summary>
public class TaxBalanceAction
{
    /// <summary>
    /// The description of the action.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ProcessDateKind KindBeginOfTaxationPeriod { get; set; }

    /// <summary>
    /// The beginning date of the process. It is active if <see cref="KindBeginOfTaxationPeriod"/> is <see cref="ProcessDateKind.Custom"/>.
    /// </summary>
    public DateOnly? BeginOfTaxationPeriod { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ProcessDateKind KindEndOfTaxationPeriod { get; set; }

    /// <summary>
    /// The date of the process. It is active if <see cref="KindEndOfTaxationPeriod"/> is <see cref="ProcessDateKind.Custom"/>.
    /// </summary>
    public DateOnly? EndOfTaxationPeriod { get; set; }

    /// <summary>
    /// The sequence of the action. It is used to determine the order of the actions occurred at the same date.
    /// </summary>
    public int? Sequence { get; set; }

    /// <summary>
    /// The tax factor that will be applied to the balance of the source account.
    /// For example, if the balance factor is 0.1, the tax amount will be 10% of the balance of the source account.
    /// This may help in case of the capital benefits tax.
    /// </summary>
    public decimal BalanceFactor { get; set; }

    /// <summary>
    /// The tax type of the accounts that will be affected by this action.
    /// </summary>
    public TaxType TaxType { get; set; }
}
