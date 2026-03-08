using Application.Features.ContributionCalculator.Models;
using LanguageExt;

namespace Application.Features.ContributionCalculator;

public interface ICompoundingContributionCalculator
{
    Either<string, ContributionResult> Calculate(ContributionRequest request);

    Either<string, ContributionResult> CalculateFixedYearlyContribution(
        DateTime startDate,
        decimal initialAmount,
        decimal yearlyContributionAmount,
        int contributionCount,
        bool investAtBeginningOfYear,
        decimal annualInterestRate,
        CompoundingFrequency compoundingFrequency,
        bool returnSequence = false);

    Either<string, ContributionResult> CalculateYearlyContributionUntilFinalDate(
        DateTime startDate,
        DateTime finalDate,
        decimal initialAmount,
        decimal yearlyContributionAmount,
        bool investAtBeginningOfYear,
        decimal annualInterestRate,
        CompoundingFrequency compoundingFrequency,
        bool returnSequence = false);
}
