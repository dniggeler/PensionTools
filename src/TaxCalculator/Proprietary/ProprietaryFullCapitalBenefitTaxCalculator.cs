using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionCoach.Tools.TaxCalculator.Proprietary
{
    public class ProprietaryFullCapitalBenefitTaxCalculator : IFullCapitalBenefitTaxCalculator
    {
        private readonly Func<Canton, ICapitalBenefitTaxCalculator> capitalBenefitCalculatorFunc;
        private readonly IFederalCapitalBenefitTaxCalculator federalCalculator;
        private readonly IMapper mapper;

        public ProprietaryFullCapitalBenefitTaxCalculator(
            Func<Canton, ICapitalBenefitTaxCalculator> capitalBenefitCalculatorFunc,
            IFederalCapitalBenefitTaxCalculator federalCalculator,
            IMapper mapper)
        {
            this.capitalBenefitCalculatorFunc = capitalBenefitCalculatorFunc;
            this.federalCalculator = federalCalculator;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
            int calculationYear,
            MunicipalityModel municipality,
            CapitalBenefitTaxPerson capitalBenefitTaxPerson,
            bool withMaxAvailableCalculationYear)
        {
            var maxCalculationYear = withMaxAvailableCalculationYear
                ? Math.Min(calculationYear, 2019)
                : calculationYear;

            var capitalBenefitTaxResultTask =
                capitalBenefitCalculatorFunc(municipality.Canton)
                    .CalculateAsync(maxCalculationYear, municipality.BfsNumber, municipality.Canton, capitalBenefitTaxPerson);

            var federalTaxPerson =
                mapper.Map<FederalTaxPerson>(capitalBenefitTaxPerson);

            var federalTaxResultTask =
                federalCalculator.CalculateAsync(maxCalculationYear, federalTaxPerson);

            await Task.WhenAll(capitalBenefitTaxResultTask, federalTaxResultTask);

            var sb = new StringBuilder();

            var stateTaxResult = await capitalBenefitTaxResultTask;
            var federalTaxResult = await federalTaxResultTask;

            stateTaxResult.MapLeft(r => sb.AppendLine(r));
            federalTaxResult.MapLeft(r => sb.AppendLine(r));

            var fullResult =
                from s in stateTaxResult.ToOption()
                from f in federalTaxResult.ToOption()
                select new FullCapitalBenefitTaxResult
                {
                    StateResult = s,
                    FederalResult = f,
                };

            return fullResult
                .Match<Either<string, FullCapitalBenefitTaxResult>>(
                    Some: r => r,
                    None: () => sb.ToString());
        }
    }
}
