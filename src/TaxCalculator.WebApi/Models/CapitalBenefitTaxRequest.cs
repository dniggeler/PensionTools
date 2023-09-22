using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace TaxCalculator.WebApi.Models;

public class CapitalBenefitTaxRequest
{
    [MaxLength(50)]
    public string Name { get; set; }

    [Range(2018, 2099, ErrorMessage = "Valid tax years start from 2018")]
    public int CalculationYear { get; set; }

    public CivilStatus CivilStatus { get; set; }

    public ReligiousGroupType ReligiousGroup { get; set; }

    public ReligiousGroupType? PartnerReligiousGroup { get; set; }

    [Required]
    [Range(typeof(int), "0", "10000", ErrorMessage = "BFS number not valid")]
    public int BfsMunicipalityId { get; set; }

    [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
    public decimal TaxableBenefits { get; set; }
}
