using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Tax.Tools.Comparison.Abstractions;

namespace Tax.Tools.Comparison
{
    public class TaxComparer : ITaxComparer
    {
        private readonly IFullCapitalBenefitTaxCalculator capitalBenefitCalculator;

        public TaxComparer(IFullCapitalBenefitTaxCalculator capitalBenefitCalculator)
        {
            this.capitalBenefitCalculator = capitalBenefitCalculator;
        }

        public async Task<Either<string, IReadOnlyCollection<FullCapitalBenefitTaxResult>>> CompareCapitalBenefitTaxAsync(int calculationYear, int municipalityId, Canton canton, CapitalBenefitTaxPerson person)
        {
            Dictionary<int, Canton> allMunicipalities
                = new Dictionary<int, Canton>
                {
                    { 2526, Canton.SO },
                    { 261, Canton.ZH },
                };

            List<FullCapitalBenefitTaxResult> resultList =
                new List<FullCapitalBenefitTaxResult>();

            foreach (var kp in allMunicipalities)
            {
                var result =
                    await this.capitalBenefitCalculator
                        .CalculateAsync(
                            calculationYear,
                            kp.Key,
                            kp.Value,
                            person);

                result
                    .IfRight(r => resultList.Add(r));
            }

            return resultList;
        }
    }
}