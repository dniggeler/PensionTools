using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator
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
            int municipalityId,
            Canton canton,
            CapitalBenefitTaxPerson capitalBenefitTaxPerson)
        {
            var capitalBenefitTaxResultTask =
                this.capitalBenefitCalculatorFunc(canton)
                    .CalculateAsync(calculationYear, municipalityId, canton, capitalBenefitTaxPerson);

            var federalTaxPerson =
                this.mapper.Map<FederalTaxPerson>(capitalBenefitTaxPerson);

            var federalTaxResultTask =
                this.federalCalculator.CalculateAsync(calculationYear, federalTaxPerson);

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