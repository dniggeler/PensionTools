using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Tax.Tools.Comparison.Abstractions;

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

        public async Task<Either<string, Dictionary<int ,FullCapitalBenefitTaxResult>>> CompareCapitalBenefitTaxAsync(int calculationYear, int municipalityId, Canton canton, CapitalBenefitTaxPerson person)
        {
            IReadOnlyCollection<TaxSupportedMunicipalityModel> municipalities =
                await this.municipalityConnector
                    .GetAllSupportTaxCalculationAsync(calculationYear);

            var resultList =
                new Dictionary<int, FullCapitalBenefitTaxResult>();

            foreach (var municipality in municipalities
                .Where(item => item.BfsNumber != municipalityId))
            {
                var result =
                    await this.capitalBenefitCalculator
                        .CalculateAsync(
                            calculationYear,
                            municipality.BfsNumber,
                            municipality.Canton,
                            person);

                result
                    .IfRight(r =>
                        resultList.Add(municipality.BfsNumber, r));
            }

            return resultList;
        }
    }
}