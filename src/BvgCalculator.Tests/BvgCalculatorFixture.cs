using System;
using System.Threading.Tasks;
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

        public async Task<BvgBaseFactorResult> GetBvgBaseFactorsAsync(BvgPerson person, DateTime processDate)
        {
            BvgProcessActuarialReserve predecessor =
                await GetActuarialReserveAsync(processDate, person);

            return await _calculator.CalculateAsync(predecessor, processDate, person);
        }

        public async Task<BvgCalculationResult> GetBvgBenefitsAsync(
            BvgPerson person, DateTime processDate)
        {
            BvgProcessActuarialReserve predecessor =
                await GetActuarialReserveAsync(processDate, person);

            return await GetBvgBenefitsAsync(person, processDate, predecessor);
        }

        public async Task<BvgCalculationResult> GetBvgBenefitsAsync(
            BvgPerson person, DateTime processDate, BvgProcessActuarialReserve actuarialReserve)
        {
            return await _calculator.CalculateRawValuesAsync(actuarialReserve, processDate, person);
        }

        public BvgPerson GetTestPerson(DateTime birthdate, decimal reportedSalary = 100_000, decimal partTimeDegree = 1)
        {
            return GetCurrentPersonDetails(birthdate, reportedSalary, partTimeDegree);
        }

        public BvgPerson GetTestPersonBelowSalaryThreshold()
        {
            BvgPerson personDetails = GetCurrentPersonDetails(new DateTime(1974, 8, 31), 20_000, 1M);

            return personDetails;
        }

        private Task<BvgProcessActuarialReserve> GetActuarialReserveAsync(
            DateTime processDate, BvgPerson person)
        {
            BvgProcessActuarialReserve predecessorProcess = new BvgProcessActuarialReserve();

            return GetActuarialReserveAsync(processDate, person, predecessorProcess);
        }

        private async Task<BvgProcessActuarialReserve> GetActuarialReserveAsync(
            DateTime processDate, BvgPerson person, BvgProcessActuarialReserve predecessorProcess)
        {
            (decimal CapitalBoY, decimal CapitalEoY) retirementCapitalCurrentYearTuple =
                await _calculator.CalculateRetirementCapitalCurrentYearAsync(predecessorProcess, processDate, person);

            return new BvgProcessActuarialReserve
            {
                DateOfProcess = processDate,
                Dkx = retirementCapitalCurrentYearTuple.CapitalBoY,
                Dkx1 = retirementCapitalCurrentYearTuple.CapitalEoY
            };
        }

        private BvgPerson GetCurrentPersonDetails(DateTime birthdate, decimal salary, decimal partTimeDegree)
        {
            return new BvgPerson
            {
                DateOfBirth = birthdate,
                Gender = Gender.Male,
                HasStop = false,
                HasStopWithRiskProtection = false,
                PartTimeDegree = partTimeDegree,
                ReportedSalary = salary,
                WorkingAbilityDegree = 1,
                DisabilityDegree = 0,
            };
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
    }
}
