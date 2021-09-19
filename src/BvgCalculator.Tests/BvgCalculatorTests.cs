using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LanguageExt;
using Newtonsoft.Json;
using PensionCoach.Tools.BvgCalculator.Models;
using PensionCoach.Tools.CommonTypes;
using Snapshooter.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace BvgCalculator.Tests
{
    [Trait("BVG", "Calculator")]
    public class BvgCalculatorTests : IClassFixture<BvgCalculatorFixture>
    {
        private readonly BvgCalculatorFixture _fixture;
        private readonly ITestOutputHelper _outputHelper;

        public BvgCalculatorTests(BvgCalculatorFixture fixture, ITestOutputHelper outputHelper)
        {
            _fixture = fixture;
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "BVG Result When Retiring")]
        public async Task ShouldCalculateResultWhenRetiring()
        {
            // given
            DateTime processDate = new DateTime(2019, 1, 1);
            DateTime birthdate = new DateTime(1954, 3, 13);

            BvgPerson person = _fixture.GetTestPerson(birthdate);
            person.ReportedSalary = 85000M;
            person.PartTimeDegree = 0.8M;

            // when
            Either<string, BvgCalculationResult> response =
                await _fixture.GetBvgBenefitsAsync(0, person, processDate);

            BvgCalculationResult result = response.IfLeft(err => throw new ApplicationException(err));

            // then
            result.RetirementCapitalSequence.Should().NotBeNullOrEmpty();

            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Default BVG Result")]
        public async Task ShouldCalculateDefaultResult()
        {
            // given
            DateTime processDate = new DateTime(2019, 1, 1);
            DateTime birthdate = new DateTime(1974, 8, 31);

            BvgPerson person = _fixture.GetTestPerson(birthdate);

            // when
            var response =
                await _fixture.GetBvgBenefitsAsync(0, person, processDate);

            BvgCalculationResult result = response.IfLeft(err => throw new ApplicationException(err));

            // then
            result.RetirementCapitalSequence.Should().NotBeNullOrEmpty();

            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Calculate Benefits")]
        public async Task ShouldReturnBenefitsCalculationResult()
        {
            // given
            BvgCalculationResult expectedResult = new BvgCalculationResult
            {
                DateOfRetirement = new DateTime(2039, 9, 1),
                EffectiveSalary = 100000,
                InsuredSalary = 60435M,
                RetirementCredit = 9065.25M,
                RetirementCreditFactor = 0.15M,
                RetirementCapitalEndOfYear = 9065.0M,
                FinalRetirementCapital = 227286,
                FinalRetirementCapitalWithoutInterest = 206687,
                PartnerPension = 8433,
                OrphanPension = 2811,
                ChildPensionForDisabled = 2811,
                DisabilityPension = 14055,
                RetirementPension = 15455
            };

            DateTime processDate = new DateTime(2019, 1, 1);
            DateTime birthdate = new DateTime(1974, 8, 31);

            BvgPerson person = _fixture.GetTestPerson(birthdate);

            // when
            var response =
                await _fixture.GetBvgBenefitsAsync(9065, person, processDate);

            BvgCalculationResult result = response.IfLeft(err => throw new ApplicationException(err));

            // then
            result.RetirementCapitalSequence.Should().NotBeNullOrEmpty();
            result.RetirementCreditSequence.Should().NotBeNullOrEmpty();

            result.Should().BeEquivalentTo(expectedResult,
                o => o.Excluding(obj => obj.RetirementCapitalSequence)
                    .Excluding(obj => obj.RetirementCreditSequence));

            _outputHelper.WriteLine(JsonConvert.SerializeObject(result));
        }

        [Fact(DisplayName = "Benefits If Below Salary Threshold")]
        public async Task ShouldReturnBenefitsIfBelowSalaryThreshold()
        {
            // given
            BvgCalculationResult expectedResult = new BvgCalculationResult
            {
                DateOfRetirement = new DateTime(2039, 9, 1),
                EffectiveSalary = 20000M,
                InsuredSalary = 0M,
                RetirementCredit = 0M,
                RetirementCreditFactor = 0.15M,
            };

            DateTime processDate = new DateTime(2019, 1, 1);

            BvgPerson person = _fixture.GetCurrentPersonDetails(new DateTime(1974, 8, 31), 20_000, 1M);

            // when
            var response =
                await _fixture.GetBvgBenefitsAsync(0, person, processDate);

            BvgCalculationResult result = response.IfLeft(err => throw new ApplicationException(err));

            // then
            result.RetirementCapitalSequence.Should().NotBeNullOrEmpty();
            result.RetirementCreditSequence.Should().NotBeNullOrEmpty();

            result.Should().BeEquivalentTo(expectedResult, options => options.Excluding(o => o.RetirementCapitalSequence)
                 .Excluding(o => o.RetirementCreditSequence));

            _outputHelper.WriteLine(JsonConvert.SerializeObject(result));
        }

        [Theory(DisplayName = "BVG Benefits")]
        [MemberData(nameof(GetTestData))]
        public async Task Calculate_Bvg_Benefits(
            string dateOfProcessString,
            decimal salary,
            string dateOfBirthString,
            int genderCode,
            decimal currentRetirementCapital,
            decimal expectedRetirementPension)
        {
            DateTime dateOfProcess = DateTime.Parse(dateOfProcessString);
            DateTime dateOfBirth = DateTime.Parse(dateOfBirthString);
            Gender gender = (Gender)genderCode;

            BvgPerson person = _fixture.GetCurrentPersonDetails(dateOfBirth, salary, 1M);
            person.Gender = gender;

            var response =
                await _fixture.GetBvgBenefitsAsync(currentRetirementCapital, person, dateOfProcess);

            BvgCalculationResult result = response.IfLeft(err => throw new ApplicationException(err));

            // then
            result.RetirementPension.Should().Be(expectedRetirementPension);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[]
                { "2017-01-01",
                    100_000,
                    "1969-03-17",
                    Gender.Male,
                    107202,
                    20610 };
        }
    }
}
