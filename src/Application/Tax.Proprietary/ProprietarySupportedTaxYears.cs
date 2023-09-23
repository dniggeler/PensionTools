using Application.Features.FullTaxCalculation;

namespace Application.Tax.Proprietary;

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
