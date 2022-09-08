using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
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
            CapitalBenefitTaxPerson person)
        {
            var validationResult = taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine =
                    string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return $"validation failed: {errorMessageLine}";
            }

            IReadOnlyCollection<TaxSupportedMunicipalityModel> municipalities =
                await municipalityConnector
                    .GetAllSupportTaxCalculationAsync();

            var resultList = new List<CapitalBenefitTaxComparerResult>();

            foreach (TaxSupportedMunicipalityModel municipality in municipalities)
            {
                MunicipalityModel municipalityModel = new MunicipalityModel()
                {
                    BfsNumber = municipality.BfsMunicipalityNumber,
                    Canton = municipality.Canton,
                    EstvTaxLocationId = null
                };

                var result =
                    await capitalBenefitCalculator
                        .CalculateAsync(
                            municipality.MaxSupportedYear,
                            municipalityModel,
                            person);

                result
                    .Map(r => new CapitalBenefitTaxComparerResult
                    {
                        MunicipalityId = municipality.BfsMunicipalityNumber,
                        MunicipalityName = municipality.Name,
                        Canton = municipality.Canton,
                        MaxSupportedTaxYear = municipality.MaxSupportedYear,
                        MunicipalityTaxResult = r,
                    })
                    .IfRight(r =>
                        resultList.Add(r));
            }

            return resultList;
        }
    }
}
