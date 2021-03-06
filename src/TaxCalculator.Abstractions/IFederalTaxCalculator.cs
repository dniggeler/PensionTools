﻿using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IFederalTaxCalculator
    {
        Task<Either<string,BasisTaxResult>> CalculateAsync(int calculationYear, FederalTaxPerson person);
    }
}
