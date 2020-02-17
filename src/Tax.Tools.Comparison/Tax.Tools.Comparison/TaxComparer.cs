using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Tax.Tools.Comparison.Abstractions;
using Tax.Tools.Comparison.Abstractions.Models;

namespace Tax.Tools.Comparison
{
    public class TaxComparer : ITaxComparer
    {
        private readonly IValidator<CapitalBenefitTaxPerson> taxPersonValidator;
        private readonly IFullCapitalBenefitTaxCalculator capitalBenefitCalculator;
        private readonly IMunicipalityConnector municipalityConnector;

        public TaxComparer(
            IValidator<CapitalBenefitTaxPerson> taxPersonValidator,
            IFullCapitalBenefitTaxCalculator capitalBenefitCalculator,
            IMunicipalityConnector municipalityConnector)
        {
            this.taxPersonValidator = taxPersonValidator;
            this.capitalBenefitCalculator = capitalBenefitCalculator;
            this.municipalityConnector = municipalityConnector;
        }

        public async Task<Either<string, IReadOnlyCollection<CapitalBenefitTaxComparerResult>>> CompareCapitalBenefitTaxAsync(
            int calculationYear, CapitalBenefitTaxPerson person)
        {
            var validationResult = this.taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine =
                    string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return $"validation failed: {errorMessageLine}";
            }

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