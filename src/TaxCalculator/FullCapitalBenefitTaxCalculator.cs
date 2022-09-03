using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionCoach.Tools.TaxCalculator
{
    public class FullCapitalBenefitTaxCalculator : IFullCapitalBenefitTaxCalculator
    {
        private readonly Func<Canton, ICapitalBenefitTaxCalculator> capitalBenefitCalculatorFunc;
        private readonly IFederalCapitalBenefitTaxCalculator federalCalculator;
        private readonly IMapper mapper;

        public FullCapitalBenefitTaxCalculator(
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
            int taxId,
            Canton canton,
            CapitalBenefitTaxPerson capitalBenefitTaxPerson,
            bool withMaxAvailableCalculationYear)
        {
            int maxCalculationYear = withMaxAvailableCalculationYear
                ? Math.Min(calculationYear, 2019)
                : calculationYear;

            var capitalBenefitTaxResultTask =
                capitalBenefitCalculatorFunc(canton)
                    .CalculateAsync(maxCalculationYear, taxId, canton, capitalBenefitTaxPerson);

            var federalTaxPerson =
                mapper.Map<FederalTaxPerson>(capitalBenefitTaxPerson);

            var federalTaxResultTask =
                federalCalculator.CalculateAsync(maxCalculationYear, federalTaxPerson);

            await Task.WhenAll(capitalBenefitTaxResultTask, federalTaxResultTask);

            StringBuilder sb = new StringBuilder();

            Either<string, CapitalBenefitTaxResult> stateTaxResult = await capitalBenefitTaxResultTask;
            Either<string, BasisTaxResult> federalTaxResult = await federalTaxResultTask;

            stateTaxResult.MapLeft(r => sb.AppendLine(r));
            federalTaxResult.MapLeft(r => sb.AppendLine(r));

            Option<FullCapitalBenefitTaxResult> fullResult =
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
