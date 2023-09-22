using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Features.TaxComparison.Models;

public class CapitalBenefitTaxComparerRequest
{
    [MaxLength(50)]
    public string Name { get; set; }

    public CivilStatus CivilStatus { get; set; }

    public ReligiousGroupType ReligiousGroup { get; set; }

    public ReligiousGroupType? PartnerReligiousGroup { get; set; }

    /// <summary>
    /// List of BFS number defines for which municipalites a comparison is calculated.
    /// </summary>
    public int[] BfsNumberList { get; set; }

    [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
    public decimal TaxableBenefits { get; set; }
}
