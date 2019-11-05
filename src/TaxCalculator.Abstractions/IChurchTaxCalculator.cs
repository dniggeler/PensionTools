﻿using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IChurchTaxCalculator
    {
        Task<Either<string,ChurchTaxResult>> CalculateAsync(
            int calculationYear, ChurchTaxPerson person, SingleTaxResult taxResult);
    }
}
