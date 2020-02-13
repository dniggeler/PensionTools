using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Tax.Tools.Comparison.Abstractions;
using Tax.Tools.Comparison.Abstractions.Models;


namespace Tax.Tools.Comparison
{
    public class TaxComparer : ITaxComparer
    {
        private readonly IFullCapitalBenefitTaxCalculator capitalBenefitCalculator;
        private readonly IMunicipalityConnector municipalityConnector;

        public TaxComparer(
            IFullCapitalBenefitTaxCalculator capitalBenefitCalculator,
            IMunicipalityConnector municipalityConnector)
        {
            this.capitalBenefitCalculator = capitalBenefitCalculator;
            this.municipalityConnector = municipalityConnector;
        }

        public async Task<Either<string, IReadOnlyCollection<CapitalBenefitTaxComparerResult>>> CompareCapitalBenefitTaxAsync(
            int calculationYear, CapitalBenefitTaxPerson person)
        {
            IReadOnlyCollection<TaxSupportedMunicipalityModel> municipalities =
                await this.municipalityConnector
                    .GetAllSupportTaxCalculationAsync(calculationYear);

            var resultList = new List<CapitalBenefitTaxComparerResult>();

            foreach (var municipality in municipalities)
            {
                var result =
                    await this.capitalBenefitCalculator
                        .CalculateAsync(
                            calculationYear,
                            municipality.BfsNumber,
                            municipality.Canton,
                            person);

                result
                    .Map(r => new CapitalBenefitTaxComparerResult
                    {
                        MunicipalityId = municipality.BfsNumber,
                        MunicipalityName = municipality.Name,
                        Canton = municipality.Canton,
                        MunicipalityTaxResult = r,
                    })
                    .IfRight(r =>
                        resultList.Add(r));
            }

            return resultList;
        }
    }
}