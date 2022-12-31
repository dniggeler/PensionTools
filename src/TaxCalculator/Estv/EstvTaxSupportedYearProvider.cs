using System.Linq;
using PensionCoach.Tools.TaxCalculator.Abstractions;

namespace PensionCoach.Tools.TaxCalculator.Estv;

public class EstvTaxSupportedYearProvider : ITaxSupportedYearProvider
{
    private readonly int[] supportedTaxYears = { 2019, 2020, 2021, 2022 };

    public int[] GetSupportedTaxYears()
    {
        return supportedTaxYears;
    }

    public int MapToSupportedYear(int taxYear)
    {
        return GetSupportedTaxYears().Max();
    }
}
