using System;
using System.Collections.Generic;
using Application.Bvg.Models;
using Domain.Enums;
using Domain.Models.Bvg;
using FluentAssertions;
using LanguageExt;
using Snapshooter.Xunit;
using Xunit;

namespace BvgCalculator.Tests;

[Trait("BVG", "Calculator")]
public class BvgRevisionCalculatorTests : IClassFixture<BvgCalculatorFixture<Application.Bvg.BvgRevisionCalculator>>
{
    private readonly BvgCalculatorFixture<Application.Bvg.BvgRevisionCalculator> _fixture;

    public BvgRevisionCalculatorTests(BvgCalculatorFixture<Application.Bvg.BvgRevisionCalculator> fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "BVG Result When Retiring")]
    public void ShouldCalculateResultWhenRetiring()
    {
        // given
        DateTime processDate = new DateTime(2024, 1, 1);
        DateTime birthdate = new DateTime(1959, 3, 13);
        decimal retirementCapitalEndOfYear = 0;

        BvgPerson person = _fixture.GetTestPerson(birthdate);
        person.ReportedSalary = 85000M;
        person.PartTimeDegree = 0.8M;

        // when
        Either<string, BvgCalculationResult> response = _fixture.GetBvgBenefits(retirementCapitalEndOfYear, person, processDate);

        BvgCalculationResult result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        result.RetirementCapitalSequence.Should().NotBeNullOrEmpty();

        Snapshot.Match(result);
    }

    [Fact(DisplayName = "Default BVG Result")]
    public void ShouldCalculateDefaultResult()
    {
        // given
        DateTime processDate = new DateTime(2024, 1, 1);
        DateTime birthdate = new DateTime(1969, 12, 17);
        decimal retirementCapitalEndOfYear = 0;

        BvgPerson person = _fixture.GetTestPerson(birthdate);

        // when
        var response = _fixture.GetBvgBenefits(retirementCapitalEndOfYear, person, processDate);

        BvgCalculationResult result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        result.RetirementCapitalSequence.Should().NotBeNullOrEmpty();

        Snapshot.Match(result);
    }

    [Fact(DisplayName = "Calculate Benefits")]
    public void ShouldReturnBenefitsCalculationResult()
    {
        // given
        DateTime processDate = new DateTime(2024, 1, 1);
        DateTime birthdate = new DateTime(1974, 8, 31);

        BvgPerson person = _fixture.GetTestPerson(birthdate);

        // when
        var response = _fixture.GetBvgBenefits(0, person, processDate);

        BvgCalculationResult result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        Snapshot.Match(result);
    }

    [Fact(DisplayName = "Benefits If Below Salary Threshold")]
    public void ShouldReturnBenefitsIfBelowSalaryThreshold()
    {
        // given
        DateTime processDate = new DateTime(2024, 1, 1);

        BvgPerson person = _fixture.GetCurrentPersonDetails(new DateTime(1974, 8, 31), 20_000, 1M);

        // when
        var response = _fixture.GetBvgBenefits(0, person, processDate);

        BvgCalculationResult result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        result.RetirementCapitalSequence.Should().NotBeNullOrEmpty();
        result.RetirementCreditSequence.Should().NotBeNullOrEmpty();

        Snapshot.Match(result);
    }

    [Theory(DisplayName = "BVG Benefits")]
    [MemberData(nameof(GetTestData))]
    public void Calculate_Bvg_Benefits(
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

        var response = _fixture.GetBvgBenefits(currentRetirementCapital, person, dateOfProcess);

        BvgCalculationResult result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        result.RetirementPension.Should().BeApproximately(expectedRetirementPension, 1M);
    }

    [Theory(DisplayName = "Insured Salary")]
    [InlineData("2024-01-01", 100_000, "1969-03-17", 1, 62475)]
    [InlineData("2024-01-01", 20_000, "1969-03-17", 1, 0)]
    [InlineData("2024-01-01", 18_000, "1969-03-17", 1, 0)]
    [InlineData("2026-01-01", 100_000, "1969-03-17", 1, 70560)]
    [InlineData("2026-01-01", 20_000, "1969-03-17", 1, 16000)]
    [InlineData("2026-01-01", 18_000, "1969-03-17", 1, 0)]
    public void Calculate_Insured_Salary(
        string dateOfProcessString,
        decimal effectiveSalary,
        string dateOfBirthString,
        int genderCode,
        decimal expectedInsuredSalary)
    {
        DateTime dateOfProcess = DateTime.Parse(dateOfProcessString);
        DateTime dateOfBirth = DateTime.Parse(dateOfBirthString);
        Gender gender = (Gender)genderCode;

        BvgPerson person = _fixture.GetCurrentPersonDetails(dateOfBirth, effectiveSalary, 1M);
        person.Gender = gender;

        Either<string, decimal> response = _fixture.Calculator().InsuredSalary(dateOfProcess, person);

        decimal result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        result.Should().Be(expectedInsuredSalary);
    }

    [Fact(DisplayName = "Salary Array for BVG Maximum")]
    public void Calculate_Insured_Salaries_For_BVG_Maximum()
    {
        // given
        DateTime dateOfProcess = new(2024, 1, 1);
        DateTime dateOfBirth = new(1969, 12, 17);
        Gender gender = Gender.Male;
        decimal reportedSalary = 100_000;

        BvgPerson person = _fixture.GetCurrentPersonDetails(dateOfBirth, reportedSalary, 1M);
        person.Gender = gender;

        Either<string, BvgTimeSeriesPoint[]> response = _fixture.Calculator().InsuredSalaries(dateOfProcess, person);

        BvgTimeSeriesPoint[] result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        result.Should().NotBeNullOrEmpty();
        Snapshot.Match(result);
    }

    [Fact(DisplayName = "Salary Array For Between Thresholds")]
    public void Calculate_Insured_Salaries_Between_Threshold()
    {
        // given
        DateTime dateOfProcess = new(2024, 1, 1);
        DateTime dateOfBirth = new(1974, 8, 31);
        Gender gender = Gender.Female;
        decimal reportedSalary = 20_000;

        BvgPerson person = _fixture.GetCurrentPersonDetails(dateOfBirth, reportedSalary, 1M);
        person.Gender = gender;

        Either<string, BvgTimeSeriesPoint[]> response = _fixture.Calculator().InsuredSalaries(dateOfProcess, person);

        BvgTimeSeriesPoint[] result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        result.Should().NotBeNullOrEmpty();
        Snapshot.Match(result);
    }

    [Fact(DisplayName = "Retirement Credits Array")]
    public void Calculate_Retirement_Credits_Array()
    {
        // given
        DateTime dateOfProcess = new(2024, 1, 1);
        DateTime dateOfBirth = new(1969, 12, 17);
        Gender gender = Gender.Male;
        decimal reportedSalary = 100_000;

        BvgPerson person = _fixture.GetCurrentPersonDetails(dateOfBirth, reportedSalary, 1M);
        person.Gender = gender;

        Either<string, BvgTimeSeriesPoint[]> response = _fixture.Calculator().RetirementCredits(dateOfProcess, person);

        BvgTimeSeriesPoint[] result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        result.Should().NotBeNullOrEmpty();
        Snapshot.Match(result);
    }

    [Fact(DisplayName = "Retirement Credit Factors Array")]
    public void Calculate_Retirement_Credit_Factors_Array()
    {
        // given
        DateTime dateOfProcess = new(2024, 1, 1);
        DateTime dateOfBirth = new(1969, 12, 17);
        Gender gender = Gender.Male;
        decimal reportedSalary = 100_000;

        BvgPerson person = _fixture.GetCurrentPersonDetails(dateOfBirth, reportedSalary, 1M);
        person.Gender = gender;

        Either<string, BvgTimeSeriesPoint[]> response = _fixture.Calculator().RetirementCreditFactors(dateOfProcess, person);

        BvgTimeSeriesPoint[] result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        result.Should().NotBeNullOrEmpty();
        Snapshot.Match(result);
    }

    [Fact(DisplayName = "Retirement Credit Factors Array Young")]
    public void Calculate_Retirement_Credit_Factors_Array_Young()
    {
        // given
        DateTime dateOfProcess = new(2024, 1, 1);
        DateTime dateOfBirth = new(2002, 3, 17);
        Gender gender = Gender.Male;
        decimal reportedSalary = 50_000;

        BvgPerson person = _fixture.GetCurrentPersonDetails(dateOfBirth, reportedSalary, 1M);
        person.Gender = gender;

        Either<string, BvgTimeSeriesPoint[]> response = _fixture.Calculator().RetirementCreditFactors(dateOfProcess, person);

        BvgTimeSeriesPoint[] result = response.IfLeft(err => throw new ApplicationException(err));

        // then
        result.Should().NotBeNullOrEmpty();
        Snapshot.Match(result);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        yield return
        [
            "2024-01-01",
            100_000,
            "1969-03-17",
            Gender.Male,
            0,
            6622
        ];
    }
}
