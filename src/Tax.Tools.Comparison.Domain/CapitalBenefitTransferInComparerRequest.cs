using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Models.Tax;

namespace PensionCoach.Tools.TaxComparison;

public class CapitalBenefitTransferInComparerRequest
{
    [MaxLength(50)]
    public string Name { get; set; }

    [Range(2018, 2099, ErrorMessage = "Valid tax years start from 2018")]
    public int CalculationYear { get; set; }

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

    public IReadOnlyCollection<SingleTransferInModel> TransferIns { get; set;}

    /// <summary>
    /// Gets or sets yearly net return on transfer-ins.
    /// </summary>
    public decimal NetWealthReturn { get; set; }

    public decimal NetPensionCapitalReturn { get; set; }
    
    public bool WithCapitalBenefitTaxation { get; set; }

    /// <summary>
    /// Gets or sets available capital benefits.
    /// The amount when starting withdrawals does not include the previously added transfer-ins.
    /// </summary>
    public decimal CapitalBenefitsBeforeWithdrawal { get; set; }
    
    public IReadOnlyCollection<SingleTransferInModel> Withdrawals { get; set; }

}
