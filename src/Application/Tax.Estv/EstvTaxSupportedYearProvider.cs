using Application.Features.FullTaxCalculation;

namespace Application.Tax.Estv;

public class EstvTaxSupportedYearProvider : ITaxSupportedYearProvider
{
    private readonly int[] supportedTaxYears = { 2019, 2020, 2021, 2022, 2023 };

    public int[] GetSupportedTaxYears()
    {
        return supportedTaxYears;
    }

    public int MapToSupportedYear(int taxYear)
    {
        return GetSupportedTaxYears().Max();
    }
}
