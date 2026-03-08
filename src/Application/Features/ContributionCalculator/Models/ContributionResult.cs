namespace Application.Features.ContributionCalculator.Models;

public record ProjectionData(
    DateTime Date,
    decimal Balance,
    decimal CashFlow,
    decimal InterestAccrued);

public record ContributionResult(
    decimal FinalAmount,
    IEnumerable<ProjectionData>? Sequence);
