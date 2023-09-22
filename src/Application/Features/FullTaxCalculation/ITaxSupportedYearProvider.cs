namespace Application.Features.FullTaxCalculation;

public interface ITaxSupportedYearProvider
{
    int[] GetSupportedTaxYears();

    int MapToSupportedYear(int taxYear);
}
