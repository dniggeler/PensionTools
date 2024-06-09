using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace BlazorBvgRevisionApp.MyComponents.Models;

public class BvgPersonViewModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(25, ErrorMessage = "Name ist zu lang.", ErrorMessageResourceName = "error.person.name")]
    public string? Name { get; set; }

    [Required]
    public int ValidityYearCertificate { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [Required]
    public Gender Gender { get; set; }

    public decimal BvgRetirementCapitalEndOfYear { get; set; }

    public decimal FinalRetirementCapital { get; set; }

    public decimal ReportedSalary { get; set; }
}
