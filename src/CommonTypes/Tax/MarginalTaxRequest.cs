using System.ComponentModel.DataAnnotations;

namespace PensionCoach.Tools.CommonTypes.Tax
{
    public class MarginalTaxRequest
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
        public decimal TaxableAmount { get; set; }

        [Range(typeof(int), "0", "1000000", ErrorMessage = "Value not possible")]
        public int LowerSalaryLimit { get; set; }

        [Range(typeof(int), "0", "10000000", ErrorMessage = "Value not possible")]
        public int UpperSalaryLimit { get; set; }
    }
}
