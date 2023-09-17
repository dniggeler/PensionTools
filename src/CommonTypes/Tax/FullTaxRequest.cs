using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace PensionCoach.Tools.CommonTypes.Tax
{
    public class FullTaxRequest
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
    }
}
