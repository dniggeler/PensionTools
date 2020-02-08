using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using System.Collections.Generic;
using Tax.Tools.Comparison.Abstractions;

namespace Tax.Tools.Comparison
{
    public class TaxComparer : ITaxComparer
    {
        public Either<string, IReadOnlyCollection<FullCapitalBenefitTaxResult>> CompareCapitalBenefitTaxAsync(int calculationYear, int municipalityId, Canton canton, CapitalBenefitTaxPerson person)
        {
            throw new System.NotImplementedException();
        }
    }
}