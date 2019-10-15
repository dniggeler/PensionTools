using System.Threading.Tasks;


namespace CapitalVersusPension.Abstractions
{
    public interface IIncomeTaxCalculator
    {
        Task<TaxResult> CalculateAsync(TaxPerson taxPerson);
    }
}