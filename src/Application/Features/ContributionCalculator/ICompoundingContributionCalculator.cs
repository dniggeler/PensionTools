using Application.Features.ContributionCalculator.Models;
using LanguageExt;

namespace Application.Features.ContributionCalculator;

public interface ICompoundingContributionCalculator
{
    Either<string, ContributionResult> Calculate(ContributionRequest request);
}
