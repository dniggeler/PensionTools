using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Tools.Comparison.Abstractions;
using Xunit;

namespace Tax.Tools.Comparison.Tests
{
    [Trait("Comparer", "Capital Benefits")]
    public class CapitalBenefitTaxComparerTests
        : IClassFixture<TaxComparerFixture<ITaxComparer>>
    {
        private readonly TaxComparerFixture<ITaxComparer> fixture;

        public CapitalBenefitTaxComparerTests(TaxComparerFixture<ITaxComparer> fixture)
        {
            this.fixture = fixture;
        }

        [Fact(DisplayName = "Compare Capital Benefit Tax")]
        public async Task ShouldReturnCapitalBenefitTaxComparison()
        {
            // given
            string name = "Burli";
            int calculationYear = 2019;
            Canton canton = Canton.ZH;
            CivilStatus status = CivilStatus.Single;
            ReligiousGroupType religiousGroup = ReligiousGroupType.Protestant;

            var taxPerson = new CapitalBenefitTaxPerson
            {
                Name = name,
                CivilStatus = status,
                ReligiousGroupType = religiousGroup,
                TaxableBenefits = 2000_000,
            };

            // when
            Either<string, IReadOnlyCollection<FullCapitalBenefitTaxResult>> result =
                await fixture.Calculator.CompareCapitalBenefitTaxAsync(
                    calculationYear, 0, canton, taxPerson);
        }
    }
}
