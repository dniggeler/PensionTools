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

    public Either<string, BvgCalculationResult> GetBvgBenefits(decimal currentRetirementCapital, BvgPerson person, DateTime processDate)
    {
        return _calculator.Calculate(processDate.Year, currentRetirementCapital, person);
    }
}
