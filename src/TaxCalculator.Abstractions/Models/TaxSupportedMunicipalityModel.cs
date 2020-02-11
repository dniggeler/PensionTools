using System;
using System.Collections.Generic;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class TaxSupportedMunicipalityModel
        : IEqualityComparer<TaxSupportedMunicipalityModel>
    {
        public int BfsNumber { get; set; }

        public string Name { get; set; }

        public Canton Canton { get; set; }

        public bool Equals(TaxSupportedMunicipalityModel x, TaxSupportedMunicipalityModel y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return x.BfsNumber == y.BfsNumber;
        }

        public int GetHashCode(TaxSupportedMunicipalityModel obj)
        {
            return obj.BfsNumber.GetHashCode();
        }
    }
}