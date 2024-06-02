using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Proprietary.Contracts;

public interface IStateTaxCalculator
{
    /// <summary>
    /// Calculates state (canton) tax asynchronously.
    /// </summary>
    /// <param name="calculationYear">The calculation year.</param>
    /// <param name="municipalityId">The BFS municipality identifier.</param>
    /// <param name="canton">The canton.</param>
    /// <param name="person">The person.</param>
    /// <returns></returns>
    Task<Either<string, StateTaxResult>> CalculateAsync(
        int calculationYear,
        int municipalityId,
        Canton canton,
        TaxPerson person);
}
