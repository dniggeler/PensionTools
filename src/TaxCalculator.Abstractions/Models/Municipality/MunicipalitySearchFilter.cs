using System;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Municipality
{
    public class MunicipalitySearchFilter
    {
        public Canton Canton { get; set; }

        public string Name { get; set; }

        public int? YearOfValidity { get; set; }
    }
}