using System;
using System.ComponentModel.DataAnnotations;
using LanguageExt;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.TypeClasses;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator.WebApi.Models
{
    public class FullTaxRequest
    {
        [MaxLength(50)]
        public string Name { get; set; }

        [Range(2018, 2099, ErrorMessage = "Valid tax years start from 2018")]
        public int CalculationYear { get; set; }

        [MaxLength(2)]
        public string Canton { get; set; }

        public CivilStatus CivilStatus { get; set; }

        public ReligiousGroupType ReligiousGroup { get; set; }

        public ReligiousGroupType? PartnerReligiousGroup { get; set; }

        [MaxLength(50)]
        public string Municipality { get; set; }

        [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
        public decimal TaxableIncome { get; set; }

        [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
        public decimal TaxableFederalIncome { get; set; }

        [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "No negative values allowed")]
        public decimal TaxableWealth { get; set; }
    }
}