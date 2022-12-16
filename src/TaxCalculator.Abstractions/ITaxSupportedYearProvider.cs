namespace PensionCoach.Tools.TaxCalculator.Abstractions;

public interface ITaxSupportedYearProvider
{
    int[] GetSupportedTaxYears();

    int MapToSupportedYear(int taxYear);
}
