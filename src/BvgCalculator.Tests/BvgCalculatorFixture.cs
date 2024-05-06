using System;
using System.Threading.Tasks;
using Application.Bvg;
using Application.Extensions;
using Domain.Enums;
using Domain.Models.Bvg;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;

namespace BvgCalculator.Tests;

public class BvgCalculatorFixture<T> where T : IBvgCalculator
{
    private readonly T _calculator;

    public BvgCalculatorFixture()
    {
        ServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddBvgCalculators();
        ServiceProvider provider = serviceCollection.BuildServiceProvider();

        _calculator = provider.GetRequiredService<T>();
    }

    internal BvgPerson GetCurrentPersonDetails(DateTime birthdate, decimal salary, decimal partTimeDegree)
    {
        return new BvgPerson
        {
            DateOfBirth = birthdate,
            Gender = Gender.Male,
            PartTimeDegree = partTimeDegree,
            ReportedSalary = salary,
            DisabilityDegree = 0,
        };
    }

    public T Calculator()
    {
        return _calculator;
    }

    public BvgPerson GetTestPerson(
        DateTime birthdate, decimal reportedSalary = 100_000, decimal partTimeDegree = 1)
    {
        return GetCurrentPersonDetails(birthdate, reportedSalary, partTimeDegree);
    }

    public BvgPerson GetDefaultPerson()
    {
        BvgPerson personDetails = new BvgPerson
        {
            DateOfBirth = new DateTime(1974, 8, 31),
            Gender = Gender.Male,
            PartTimeDegree = 1.0M,
            ReportedSalary = 100000,
        };

        return personDetails;
    }

    public Task<Either<string, BvgCalculationResult>> GetBvgBenefitsAsync(
        decimal currentRetirementCapital, BvgPerson person, DateTime processDate)
    {
        var predecessorCapital = new PredecessorRetirementCapital
        {
            DateOfProcess = processDate,
            BeginOfYearAmount = 0,
            CurrentAmount = 0,
            EndOfYearAmount = currentRetirementCapital,
        };
            
        return _calculator.CalculateAsync(predecessorCapital, processDate, person);
    }
}
