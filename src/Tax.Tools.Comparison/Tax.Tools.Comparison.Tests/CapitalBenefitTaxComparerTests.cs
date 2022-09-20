﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using Snapshooter.Xunit;
using Tax.Tools.Comparison.Abstractions;
using Tax.Tools.Comparison.Abstractions.Models;
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
            var result = await fixture.Calculator.CompareCapitalBenefitTaxAsync(taxPerson);

            // then
            Assert.True(result.IsRight);
            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Compare Capital Benefit Tax (streamed)")]
        public async Task ShouldReturnCapitalBenefitTaxComparisonByStreaming()
        {
            // given
            const int maxNumberOfMunicipality = 20;

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
            await foreach (var compareResult in fixture.Calculator.CompareCapitalBenefitTaxAsync(taxPerson, maxNumberOfMunicipality))
            {
                results.Add(compareResult);
            }

            Assert.All(results, m => Assert.True(m.IsRight));

            List<CapitalBenefitTaxComparerResult> orderedResults = new();
            results.Iter(m => m.IfRight(r => orderedResults.Add(r)));

            // then
            Snapshot.Match(orderedResults.OrderBy(item => item.MunicipalityName));
        }
    }
}
