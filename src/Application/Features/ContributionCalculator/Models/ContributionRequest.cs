namespace Application.Features.ContributionCalculator.Models;

public record ContributionRequest(
    DateTime StartDate,
    decimal InitialAmount,
    IEnumerable<Contribution> Contributions,
    decimal AnnualInterestRate,
    DateTime FinalDate,
    CompoundingFrequency CompoundingFrequency,
    bool ReturnSequence = false);
