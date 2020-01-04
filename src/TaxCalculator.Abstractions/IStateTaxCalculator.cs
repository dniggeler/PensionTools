﻿using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IStateTaxCalculator
    {
        /// <summary>
        /// Calculates state (canton) tax asynchronously.
        /// </summary>
        /// <param name="calculationYear">The calculation year.</param>
        /// <param name="municipalityId">The BFS municipality identifier.</param>
        /// <param name="person">The person.</param>
        /// <returns></returns>
        Task<Either<string,StateTaxResult>> CalculateAsync(
            int calculationYear,
            int municipalityId,
            TaxPerson person);
    }
}
