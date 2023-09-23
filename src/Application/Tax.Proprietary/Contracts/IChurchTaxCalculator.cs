using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Proprietary.Contracts;

public interface IChurchTaxCalculator
{
    /// <summary>
    /// Calculates the church tax for given year and municipality.
    /// </summary>
    /// <param name="calculationYear">The calculation year.</param>
    /// <param name="municipality">The municipality.</param>
    /// <param name="person">The person.</param>
    /// <param name="taxResult">The tax result.</param>
    /// <returns></returns>
    Task<Either<string, ChurchTaxResult>> CalculateAsync(
        int calculationYear,
        int municipality,
        ChurchTaxPerson person,
        AggregatedBasisTaxResult taxResult);

    /// <summary>
    /// Calculates the church tax with a given tax rate record.
    /// </summary>
    /// <param name="person">The person.</param>
    /// <param name="taxRateEntity">The tax rate entity.</param>
    /// <param name="taxResult">The tax result.</param>
    /// <returns></returns>
    Task<Either<string, ChurchTaxResult>> CalculateAsync(
        ChurchTaxPerson person,
        TaxRateEntity taxRateEntity,
        AggregatedBasisTaxResult taxResult);
}
