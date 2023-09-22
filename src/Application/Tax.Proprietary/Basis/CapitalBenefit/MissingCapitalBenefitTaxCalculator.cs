using Application.Tax.Proprietary.Abstractions;
using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Application.Tax.Proprietary.Basis.CapitalBenefit;

/// <summary>
/// Null calculator for missing capital benefit calculator.
/// </summary>
public class MissingCapitalBenefitTaxCalculator : ICapitalBenefitTaxCalculator
{
    private readonly ILogger<MissingCapitalBenefitTaxCalculator> logger;

    public MissingCapitalBenefitTaxCalculator(ILogger<MissingCapitalBenefitTaxCalculator> logger)
    {
        this.logger = logger;
    }

    public Task<Either<string, CapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear, int municipalityId, Canton canton, CapitalBenefitTaxPerson person)
    {
        string msg = $"No capital benefit tax calculator for canton {canton} available";

        Either<string, CapitalBenefitTaxResult> result = msg;

        logger.LogWarning(msg);

        return Task.FromResult(result);
    }
}
