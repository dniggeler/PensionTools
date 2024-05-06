namespace Domain.Models.Bvg;

public class RetirementCapital(DateTime date, decimal value, decimal valueWithoutInterest)
{
    /// <summary>
    /// Date of validity
    /// </summary>
    public DateTime Date { get; } = date;

    /// <summary>
    /// Mandatory retirement assets BVG portion
    /// </summary>
    public decimal Value { get; } = value;

    /// <summary>
    /// Gets the value without interest BVG by raw retirement credits.
    /// </summary>
    /// <value>
    /// The value without interest BVG by raw retirement credits.
    /// </value>
    public decimal ValueWithoutInterest { get; } = valueWithoutInterest;
}
