﻿using System.Runtime.InteropServices;
using LanguageExt;


namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class FederalTaxPerson
    {
        public string Name { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public decimal TaxableIncome { get; set; } 
        public Option<ReligiousGroupType> DenominationType { get; set; }
    }
}