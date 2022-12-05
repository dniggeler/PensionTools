using System;
using System.ComponentModel.DataAnnotations;
using PensionCoach.Tools.CommonTypes;

namespace BlazorApp.ViewModels
{
    public class PersonViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(25, ErrorMessage = "Name ist zu lang.", ErrorMessageResourceName = "error.person.name")]
        public string Name { get; set; }
        
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }
        
        public CivilStatus CivilStatus { get; set; }
        
        public int NumberOfChildren { get; set; }
        
        public ReligiousGroupType ReligiousGroupType { get; set; }
        
        public ReligiousGroupType? PartnerReligiousGroupType { get; set; }

        public decimal TaxableIncome { get; set; }

        public decimal TaxableFederalIncome => TaxableIncome;
        
        public decimal TaxableWealth { get; set; }

        public int BfsMunicipalityId { get; set; }

        public Canton Canton { get; set; }

        public string MunicipalityName { get; set; }

        public decimal FinalRetirementCapital { get; set; }

        public decimal FinalCapital3a { get; set; }
    }
}
