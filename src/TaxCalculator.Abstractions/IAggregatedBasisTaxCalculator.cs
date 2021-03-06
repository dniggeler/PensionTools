﻿using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IAggregatedBasisTaxCalculator
    {
        Task<Either<string, AggregatedBasisTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, TaxPerson person);
    }
}
