using System.Text.Json;
using Application.Bvg;
using Application.Bvg.Models;
using BlazorBvgRevisionApp.MyComponents.Models;
using BlazorBvgRevisionApp.Services.Models;
using Domain.Models.Bvg;
using LanguageExt;
using static System.Decimal;

namespace BlazorBvgRevisionApp.Services;

public class PensionPlanSimulator(
    BvgCalculator bvgCalculator,
    ISavingsProcessProjectionCalculator projectionCalculator,
    BvgRetirementDateCalculator retirementDateCalculator,
    ILogger<PensionPlanSimulator> logger)
{
    public PensionPlanSimulationResult Calculate(PensionPlanViewModel pensionPlan, BvgPersonViewModel person)
    {
        ArgumentNullException.ThrowIfNull(person.DateOfBirth);

        DateTime dateOfRetirement = retirementDateCalculator.DateOfRetirement(person.Gender, person.DateOfBirth.Value);
        TechnicalAge retirementAge = retirementDateCalculator.RetirementAge(person.Gender, person.DateOfBirth.Value);
        TechnicalAge finalAge = retirementAge;

        int yearOfBeginProjection = person.CalculationYear + 1;

        Either<string, BvgCalculationResult> bvgCalculationResult = bvgCalculator.Calculate(person.CalculationYear, person.BvgRetirementCapitalEndOfYear,
            new BvgPerson
            {
                ReportedSalary = person.ReportedSalary,
                DateOfBirth = person.DateOfBirth.Value,
                DisabilityDegree = Zero,
                Gender = person.Gender,
                PartTimeDegree = One
            });

        bvgCalculationResult.Match(
            Left: error => logger.LogError("BVG calculation error: {error}", error),
            Right: result => logger.LogInformation($"BVG calculation result: {JsonSerializer.Serialize(result)}"));

        RetirementSavingsProcessResult[] projections = projectionCalculator.ProjectionTable(
            pensionPlan.ProjectionInterestRate,
            dateOfRetirement,
            dateOfRetirement,
            retirementAge,
            finalAge,
            yearOfBeginProjection,
            pensionPlan.RetirementCapitalEndOfYear,
            RetirementCreditSelector(pensionPlan.InsuredSalary, pensionPlan.RetirementCredits));

        RetirementSavingsProcessResult? simulationResult = projections.SingleOrDefault(item => item.IsRetirementDate);

        if(simulationResult is not null)
        {
            decimal retirementPension = simulationResult.RetirementCapital * pensionPlan.ConversionRate;
            return new PensionPlanSimulationResult(simulationResult.RetirementCapital, retirementPension);
        }

        return new PensionPlanSimulationResult(null, null);
    }

    private Func<TechnicalAge, decimal> RetirementCreditSelector(decimal insuredSalary, RetirementCreditRange[] retirementCreditTable)
    {
        Dictionary<int, decimal> retirementCreditByAge = CreateRetirementCreditDictionary(retirementCreditTable);

        return age =>
        {
            if(retirementCreditByAge.TryGetValue(age.Years, out decimal retirementCredit))
            {
                return insuredSalary * retirementCredit;
            }
            return Zero;
        };
    }

    private Dictionary<int, decimal> CreateRetirementCreditDictionary(RetirementCreditRange[] pensionPlanRetirementCredits)
    {
        Dictionary<int, decimal> retirementCreditByAge = Enumerable.Range(0,100).ToDictionary(item => item, _ => Zero);

        foreach (var retirementCredit in pensionPlanRetirementCredits)
        {
            for (int age = retirementCredit.FromAge; age <= retirementCredit.ToAge; age++)
            {
                retirementCreditByAge[age] = retirementCredit.Rate;
            }
        }

        return retirementCreditByAge;
    }
}
