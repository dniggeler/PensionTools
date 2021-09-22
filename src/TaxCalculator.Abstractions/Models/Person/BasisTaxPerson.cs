﻿using LanguageExt;
using PensionCoach.Tools.CommonTypes;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public record BasisTaxPerson
    {
        public string Name { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public decimal TaxableAmount { get; set; } 
    }
}
