using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Contracts;
using Domain.Enums;
using Domain.Models.Tax.Person;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Application.Tax.Proprietary.Basis.Income;

/// <summary>
/// Null basis income calculator for missing cantons.
/// </summary>
/// <seealso cref="IBasisIncomeTaxCalculator" />
public class MissingBasisIncomeTaxCalculator : IBasisIncomeTaxCalculator
{
    private readonly ILogger<MissingBasisIncomeTaxCalculator> logger;

    public MissingBasisIncomeTaxCalculator(ILogger<MissingBasisIncomeTaxCalculator> logger)
    {
        this.logger = logger;
    }

    public Task<Either<string, BasisTaxResult>> CalculateAsync(
        int calculationYear, Canton canton, BasisTaxPerson person)
    {
        string msg = $"No income tax calculator for canton {canton.ToString()} available";

        Either<string, BasisTaxResult> result = msg;

        logger.LogWarning(msg);

        return Task.FromResult(result);
    }
}
