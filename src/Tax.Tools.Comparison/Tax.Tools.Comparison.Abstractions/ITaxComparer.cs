using System;
using System.Collections.Generic;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace Tax.Tools.Comparison.Abstractions
{
    public interface ITaxComparer
    {
        Either<string,IReadOnlyCollection<FullCapitalBenefitTaxResult>> CompareCapitalBenefitTaxAsync(
            int calculationYear, int municipalityId, Canton canton, CapitalBenefitTaxPerson person);
    }
}
