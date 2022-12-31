using System.Linq;
using PensionCoach.Tools.TaxCalculator.Abstractions;

namespace PensionCoach.Tools.TaxCalculator.Proprietary;

public class ProprietarySupportedTaxYears : ITaxSupportedYearProvider
{
    public int[] GetSupportedTaxYears()
    {
        int[] years = { 2019 };

        return years;
    }

    public int MapToSupportedYear(int taxYear)
    {
        return GetSupportedTaxYears().Max();
    }
}
