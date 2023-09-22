using System;
using Domain.Enums;

namespace PensionCoach.Tools.BvgCalculator;

/// <summary>
/// Extension methods related to date calculations in
/// the context of occupational benefits (Vorsorge)
/// </summary>
public static class OccupationalBenefitsDateExtensions
{
    public static DateTime GetRetirementDate(this DateTime birthdate, int retirementAge)
    {

        // Date of retirement
        return birthdate
            .GetBirthdateTechnical()
            .AddYears(retirementAge);
    }

    public static DateTime GetRetirementDate(this DateTime birthdate, Gender gender)
    {
        // FinalAgeByPlan BVG
        int xsBvg = Bvg.GetRetirementAge(gender);

        // Date of retirement
        return GetRetirementDate(birthdate, xsBvg);
    }

    /// <summary>
    /// Gets the birthdate technical: first day of following month.
    /// </summary>
    /// <param name="birthdate">The birthdate.</param>
    /// <returns></returns>
    public static DateTime GetBirthdateTechnical(this DateTime birthdate)
    {
        return new DateTime(birthdate.Year, birthdate.Month, 1).AddMonths(1);
    }

    public static int GetBvgAge(this DateTime birthdate, int calculationYear)
    {
        return calculationYear - birthdate.Year;
    }
}
