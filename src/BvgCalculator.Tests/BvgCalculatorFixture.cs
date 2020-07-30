﻿using System;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using PensionCoach.Tools.BvgCalculator;
using PensionCoach.Tools.BvgCalculator.Models;
using PensionCoach.Tools.CommonTypes;

namespace BvgCalculator.Tests
{
    public class BvgCalculatorFixture
    {
        private readonly IBvgCalculator _calculator;

        public BvgCalculatorFixture()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddBvgCalculators();
            ServiceProvider provider = serviceCollection.BuildServiceProvider();

            _calculator = provider.GetRequiredService<IBvgCalculator>();
        }

        internal BvgPerson GetCurrentPersonDetails(DateTime birthdate, decimal salary, decimal partTimeDegree)
        {
            return new BvgPerson
            {
                DateOfBirth = birthdate,
                Gender = Gender.Male,
                PartTimeDegree = partTimeDegree,
                ReportedSalary = salary,
                WorkingAbilityDegree = 1,
                DisabilityDegree = 0,
            };
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
                DateOfAdmittance = new DateTime(2015, 1, 1),
                Gender = Gender.Male,
                PartTimeDegree = 1.0M,
                WorkingAbilityDegree = 1.0M,
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
}
