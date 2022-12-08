using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PensionCoach.Tools.CommonTypes;

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
    public decimal NetReturnRate { get; set; }
    
    public bool WithCapitalBenefitTaxation { get; set; }

    [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
    public decimal FinalRetirementCapital { get; set; }

    [Range(typeof(decimal), "2018", "2099", ErrorMessage = "Only years in the future")]
    public int? YearOfCapitalBenefitWithdrawal { get; set; }
}
