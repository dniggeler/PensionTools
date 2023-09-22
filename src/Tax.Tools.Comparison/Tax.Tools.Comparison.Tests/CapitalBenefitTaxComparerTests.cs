using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Tax;
using Domain.Models.TaxComparison;
using LanguageExt;
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

        [Fact(DisplayName = "Compare Capital Benefit Tax (streamed)")]
        public async Task CalculateComparisonByStreaming()
        {
            // given
            int[] bfsNumbers = { 261, 3, 1344 };

            string name = "Burli";
            CivilStatus status = CivilStatus.Single;
            ReligiousGroupType religiousGroup = ReligiousGroupType.Protestant;

            var taxPerson = new CapitalBenefitTaxPerson
            {
                Name = name,
                CivilStatus = status,
                ReligiousGroupType = religiousGroup,
                TaxableCapitalBenefits = 2000_000,
            };

            // when
            List<Either<string, CapitalBenefitTaxComparerResult>> results = new List<Either<string, CapitalBenefitTaxComparerResult>>();
            await foreach (var compareResult in fixture.Calculator.CompareCapitalBenefitTaxAsync(taxPerson, bfsNumbers))
            {
                results.Add(compareResult);
            }

            Assert.All(results, m => Assert.True(m.IsRight));

            List<CapitalBenefitTaxComparerResult> orderedResults = new();
            results.Iter(m => m.IfRight(r => orderedResults.Add(r)));

            // then
            Snapshot.Match(orderedResults.OrderBy(item => item.MunicipalityName));
        }


        [Fact(DisplayName = "Compare Income and Wealth Tax")]
        public async Task CompareIncomeAndWealthTax()
        {
            // given
            int[] bfsNumbers = { 261, 3, 1344 };

            string name = "Burli";
            CivilStatus status = CivilStatus.Single;
            ReligiousGroupType religiousGroup = ReligiousGroupType.Protestant;

            var taxPerson = new TaxPerson
            {
                Name = name,
                CivilStatus = status,
                ReligiousGroupType = religiousGroup,
                TaxableFederalIncome = 100_000,
                TaxableIncome = 100_000,
                TaxableWealth = 500_000,
            };

            // when
            List<Either<string, IncomeAndWealthTaxComparerResult>> results = new ();
            await foreach (var compareResult in fixture.Calculator.CompareIncomeAndWealthTaxAsync(taxPerson, bfsNumbers))
            {
                results.Add(compareResult);
            }

            Assert.All(results, m => Assert.True(m.IsRight));

            List<IncomeAndWealthTaxComparerResult> orderedResults = new();
            results.Iter(m => m.IfRight(r => orderedResults.Add(r)));

            // then
            Snapshot.Match(orderedResults.OrderBy(item => item.MunicipalityName));
        }
    }
}
