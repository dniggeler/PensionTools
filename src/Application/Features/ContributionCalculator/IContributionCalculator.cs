using Application.Features.ContributionCalculator.Models;
using LanguageExt;

namespace Application.Features.ContributionCalculator;

public interface IContributionCalculator
{
    Either<string, ContributionResult> Calculate(ContributionRequest request);
}
