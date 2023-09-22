namespace Application.Tax.Estv.Client.Models
{
    public class SimpleCapitalTaxResult
    {
        public int TaxCanton { get; set; }
        public int TaxChurch { get; set; }
        public int TaxCity { get; set; }
        public int TaxFed { get; set; }
        public TaxLocation Location { get; set; }
    }
}
