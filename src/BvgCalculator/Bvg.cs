using System;
using System.Collections.Generic;
using Domain.Enums;
using PensionCoach.Tools.CommonUtils;

namespace PensionCoach.Tools.BvgCalculator;

/// <summary>
/// 
/// </summary>
public static class Bvg
{
    /// <summary>
    /// Definition der maximalen AHV-Altersrente 
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public static decimal GetPensionMaximum(int year)
    {
        const string errorMsg = "AHV pension value not defined for year before 2005";

        return AhvMaxPensionDictionary.Match(year, errorMsg);
    }

    /// <summary>
    /// The entry age risk
    /// </summary>
    public static readonly int EntryAgeRisk = 18;

    /// <summary>
    /// The retirement age women
    /// </summary>
    public static readonly int RetirementAgeWomen = 64;

    /// <summary>
    /// The retirement age men
    /// </summary>
    public static readonly int RetirementAgeMen = 65;

    /// <summary>
    /// Final age according BVG
    /// </summary>
    public static readonly int FinalAge = 70;

    /// <summary>
    /// Final age for orphan pension by BVG
    /// </summary>
    public static readonly int FinalAgeOrphanPension = 18;

    /// <summary>
    /// The earliest retirement age
    /// </summary>
    public static readonly int EarliestRetirementAge = 58;

    /// <summary>
    /// The entry age lob
    /// </summary>
    public static readonly int EntryAgeBvg = 25;

    /// <summary>
    /// The factor partners pension lob
    /// </summary>
    public static readonly decimal FactorPartnersPension = 0.6M;

    /// <summary>
    /// The factor child pension
    /// </summary>
    public static readonly decimal FactorChildPension = 0.2M;

    /// <summary>
    /// The FZL threshold age used in multiple conditional calculation (like FZL at age 50 or WEF withdrawals after 50)
    /// </summary>
    public static readonly int FzlThresholdAgeForWef = 50;

    /// <summary>
    /// Age related retirement credit rates
    /// </summary>  
    public static class RetirementCreditRateBvg
    {
        /// <summary>
        /// Retirement credit rates up to age 24
        /// </summary>
        public static readonly decimal UpToAge24 = 0M;

        /// <summary>
        /// Retirement credit rates LOB portion from age 25 up to 34
        /// </summary>
        public static readonly decimal UpToAge34 = .07M;

        /// <summary>
        /// Retirement credit rates LOB portion from age 35 up to 44
        /// </summary>
        public static readonly decimal UpToAge44 = .10M;

        /// <summary>
        /// Retirement credit rates LOB portion from age 45 up to 54
        /// </summary>
        public static readonly decimal UpToAge54 = .15M;

        /// <summary>
        /// Retirement credit rates LOB portion from age 55 up to final age
        /// </summary>
        public static readonly decimal UpToAgeFinal = .18M;
    }

    /// <summary>
    /// Gets the entrance threshold.
    /// </summary>
    /// <param name="ahvMax">The ahv maximum.</param>
    /// <returns></returns>
    public static decimal GetEntranceThreshold(decimal ahvMax) => SalaryEntryThresholdFactor * ahvMax;

    /// <summary>
    /// Maximum salary BVG portion
    /// </summary>
    /// <param name="ahvMax"></param>
    /// <returns></returns>
    public static decimal GetMaximumSalary(decimal ahvMax) => MultiplicationFactorBvgSalary * ahvMax;

    /// <summary>
    /// Maximal insurable salary according to BVG
    /// </summary>
    /// <param name="ahvMax"></param>
    /// <returns></returns>
    public static decimal GetMaximalInsurableSalary(decimal ahvMax) => MultiplicationFactorMaxInsurableBvgSalary * ahvMax;

    /// <summary>
    /// Minimum salary LOB portion
    /// </summary>
    /// <param name="ahvMax"></param>
    /// <returns></returns>
    public static decimal GetMinimumSalary(decimal ahvMax) => MinimalSalaryFactor * ahvMax;

    /// <summary>
    /// Coordination deduction
    /// </summary>
    /// <param name="ahvMax"></param>
    /// <returns></returns>
    public static decimal GetCoordinationDeduction(decimal ahvMax) => CoordinationDeductionFactor * ahvMax;

    /// <summary>
    /// The entry age savings
    /// </summary>
    public static readonly int EntryAgeSavings = EntryAgeBvg;

    /// <summary>
    /// Gets the retirement age.
    /// </summary>
    /// <param name="typeOfGender">The type of gender.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException">Invalid value: Gender.Undefined.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static int GetRetirementAge(Gender typeOfGender)
    {
        switch (typeOfGender)
        {
            case Gender.Undefined:
                throw new ArgumentException("Invalid value: Gender.Undefined.");
            case Gender.Female:
                return RetirementAgeWomen;
            case Gender.Male:
                return RetirementAgeMen;
            default:
                throw new ArgumentException(nameof(typeOfGender));
        }
    }

    /// <summary>
    /// BVG-Zinssatz
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public static decimal GetInterestRate(int year)
    {
        return BvgInterestRateDictionary.Match(year);
    }

    public static decimal GetUwsRateBvg(int year, Gender gender)
    {
        if (gender == Gender.Male)
        {
            return GetUwsRateBvgMale(year);
        }

        if (gender == Gender.Female)
        {
            return GetUwsRateBvgFemale(year);
        }

        throw new ArgumentException(nameof(gender));
    }

    private static readonly Dictionary<int, decimal?> AhvMaxPensionDictionary = new()
    {
        {1969, null},
        {1970, 0M},
        {1984, 0M},
        {1985, 16560M},
        {1987, 17280M},
        {1989, 18000M},
        {1991, 19200M},
        {1992, 21600M},
        {1994, 22560M},
        {1996, 23280M},
        {1998, 23880M},
        {1999, 24120M},
        {2000, 24120M},
        {2002, 24720M},
        {2004, 25320M},
        {2007, 25800M},
        {2009, 26520M},
        {2011, 27360M},
        {2013, 27840M},
        {2015, 28080M},
        {2019, 28200M},
        {2021, 28440M},
        {9999, 28680M},
    };

    private const decimal MinimalSalaryFactor = 0.125M;
    private const decimal SalaryEntryThresholdFactor = 0.75M;
    private const decimal CoordinationDeductionFactor = 0.875M;
    private const decimal MultiplicationFactorBvgSalary = 3M;
    private const decimal MultiplicationFactorMaxInsurableBvgSalary = 30M;

    private static decimal GetUwsRateBvgMale(int year)
    {
        return UwsRateMaleDictionary.Match(year);
    }

    private static decimal GetUwsRateBvgFemale(int year)
    {
        return UwsRateFemaleDictionary.Match(year);
    }

    private static readonly Dictionary<int, decimal?> UwsRateMaleDictionary =
        new Dictionary<int, decimal?>
        {
            {2010, 0.0705M},
            {2011, 0.0700M},
            {2012, 0.0695M},
            {2013, 0.069M},
            {2014, 0.0685M},
            {9999, 0.0680M},
        };

    private static readonly Dictionary<int, decimal?> UwsRateFemaleDictionary =
        new Dictionary<int, decimal?>
        {
            {2010, 0.0700M},
            {2011, 0.0695M},
            {2012, 0.0690M},
            {2013, 0.0685M},
            {2014, 0.0680M},
            {9999, 0.0680M},
        };

    private static readonly Dictionary<int, decimal?> BvgInterestRateDictionary =
        new Dictionary<int, decimal?>
        {
            {1984, 0.0M},
            {2003, 0.0400M},
            {2004, 0.0325M},
            {2005, 0.0225M},
            {2008, 0.0250M},
            {2009, 0.0275M},
            {2012, 0.0200M},
            {2014, 0.0150M},
            {2016, 0.0175M},
            {2017, 0.0125M},
            {9999, 0.0100M},
        };
}
