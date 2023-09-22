namespace Application.Tax;

public interface ITaxSupportedYearProvider
{
    int[] GetSupportedTaxYears();

    int MapToSupportedYear(int taxYear);
}
