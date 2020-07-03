using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator.Utils
{
    public static class TaxCalculatorExtensions
    {
        public static decimal GetTaxRate(this ReligiousGroupType religiousGroupType, TaxRateEntity entity)
        {
            return 0M;
        }
    }
}