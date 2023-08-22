using System.ComponentModel.DataAnnotations;
using PensionCoach.Tools.CommonTypes;

namespace PensionCoach.Tools.TaxComparison;

public class ThirdPillarVersusSelfInvestmentComparerRequest
{
    [MaxLength(50)]
    public string Name { get; set; }

    [Range(2018, 2099, ErrorMessage = "Valid tax years start from 2018")]
    public int CalculationYear { get; set; }

    /// <summary>
    /// Final transfer-in year. At this year the third-pillar account is closed and the money is transferred to the wealth.
    /// </summary>
    public int FinalYear { get; set; }

    public CivilStatus CivilStatus { get; set; }

    public ReligiousGroupType ReligiousGroup { get; set; }

    public ReligiousGroupType? PartnerReligiousGroup { get; set; }

    [Range(typeof(int), "0", "100000", ErrorMessage = "BFS Number not valid")]
    public int BfsMunicipalityId { get; set; }

    [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
    public decimal TaxableIncome { get; set; }

    [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
    public decimal TaxableFederalIncome { get; set; }

    [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
    public decimal TaxableWealth { get; set; }

    /// <summary>
    /// The amount invested in the third pillar or self-investment account.
    /// First investment is done in the calculation year up to final year.
    /// </summary>
    public decimal InvestmentAmount { get; set;}

    /// <summary>
    /// Gets or sets yearly net return on self-investment.
    /// </summary>
    public decimal NetSelfInvestmentReturn { get; set; }

    /// <summary>
    /// Gets or sets yearly net return on third pillar account.
    /// </summary>
    public decimal NetThirdPillarReturn { get; set; }
}
