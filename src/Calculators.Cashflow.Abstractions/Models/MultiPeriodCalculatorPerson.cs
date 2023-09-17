using System;
using Domain.Enums;
using PensionCoach.Tools.CommonTypes;

namespace Calculators.CashFlow.Models
{
    public record MultiPeriodCalculatorPerson
    {
        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public CivilStatus CivilStatus { get; set; }

        public int NumberOfChildren { get; set; }

        public ReligiousGroupType ReligiousGroupType { get; set; }

        public ReligiousGroupType? PartnerReligiousGroupType { get; set; }

        public Canton Canton { get; set; }

        public int MunicipalityId { get; set; }

        public decimal Income { get; set; }
        
        public decimal Wealth { get; set; }

        public (decimal Pillar3a, decimal PensionPlan) CapitalBenefits { get; set; }
        

    }
}
