using System;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace PensionCoach.Tools.CommonTypes.Bvg;

public class BvgCalculationRequest
{
    [MaxLength(50)]
    public string Name { get; set; }

    [Range(typeof(DateTime), "1/1/1970", "1/1/2099", ErrorMessage = "Valid calculation years start from 1970")]
    public DateTime DateOfCalculation { get; set; }

    [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
    public decimal RetirementCapitalBeginOfYear { get; set; }

    [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
    public decimal RetirementCapitalEndOfYear { get; set; }

    [Range(typeof(DateTime), "1/1/1900", "1/1/2099", ErrorMessage = "No valid birthdate")]
    public DateTime DateOfBirth { get; set; }

    [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
    public decimal Salary { get; set; }

    public Gender Gender { get; set; }
}
