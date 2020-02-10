using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace Tax.Tools.Comparison.Abstractions
{
    public interface ITaxComparer
    {
        Task<Either<string,Dictionary<int, FullCapitalBenefitTaxResult>>> CompareCapitalBenefitTaxAsync(
            int calculationYear, int municipalityId, Canton canton, CapitalBenefitTaxPerson person);
    }
}
