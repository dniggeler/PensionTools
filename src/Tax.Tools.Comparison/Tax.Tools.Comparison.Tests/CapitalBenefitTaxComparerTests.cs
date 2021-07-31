using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Snapshooter.Xunit;
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
            var result =
                await fixture.Calculator.CompareCapitalBenefitTaxAsync(
                    calculationYear, taxPerson);

            // then
            Assert.True(result.IsRight);
            Snapshot.Match(result);
        }
    }
}
