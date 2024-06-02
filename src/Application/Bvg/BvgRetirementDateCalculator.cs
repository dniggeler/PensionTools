using System.ComponentModel;
using Application.Extensions;
using Domain.Enums;

namespace Application.Bvg;

public class BvgRetirementDateCalculator
{
    public DateTime DateOfRetirement(Gender gender, DateTime dateOfBirth)
    {
        (int Year, int Months) finaleAge = RetirementAge(gender, dateOfBirth);

        return DateOfRetirementByAge(dateOfBirth, finaleAge);
    }

    public DateTime DateOfRetirementByAge(DateTime dateOfBirth, (int Year, int Months) finaleAge)
    {
        return dateOfBirth
            .GetBirthdateTechnical()
            .AddYears(finaleAge.Year)
            .AddMonths(finaleAge.Months);
    }

    public (int Years, int Months) RetirementAge(Gender gender, DateTime dateOfBirth)
    {
        return RetirementAgeInternal(gender, dateOfBirth);
    }

    private (int Year, int Months) RetirementAgeInternal(Gender gender, DateTime dateOfBirth)
    {
        const int lastGenerationBeforeTransition = 1960;
        const int firstTransitionGeneration = 1961;
        const int secondTransitionGeneration = 1962;
        const int thirdTransitionGeneration = 1963;
        const int additionalMonthsFirstGeneration = 3;
        const int additionalMonthsSecondGeneration = 6;
        const int additionalMonthsThirdGeneration = 9;
        const int retirementAgeFemaleBeforeTransition = 64;

        if (gender == Gender.Undefined)
        {
            throw new InvalidEnumArgumentException(nameof(Gender));
        }

        int yearOfBirth = dateOfBirth.Year;

        (int retirementAgeMen, int retirementAgeWomen) = (65, 65);

        if (gender == Gender.Female)
        {
            if (yearOfBirth > lastGenerationBeforeTransition)
            {
                return yearOfBirth switch
                {
                    firstTransitionGeneration => (retirementAgeFemaleBeforeTransition, additionalMonthsFirstGeneration),
                    secondTransitionGeneration => (retirementAgeFemaleBeforeTransition, additionalMonthsSecondGeneration),
                    thirdTransitionGeneration => (retirementAgeFemaleBeforeTransition, additionalMonthsThirdGeneration),
                    _ => (retirementAgeWomen, 0)
                };
            }

            return (retirementAgeFemaleBeforeTransition, 0);
        }

        return (retirementAgeMen, 0);
    }
}
