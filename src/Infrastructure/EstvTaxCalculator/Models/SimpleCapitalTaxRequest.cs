using System.Text.Json.Serialization;

namespace Infrastructure.EstvTaxCalculator.Models
{
    public class SimpleCapitalTaxRequest
    {
        public int TaxYear { get; set; }

        [JsonPropertyName("TaxGroupID")]
        public int TaxGroupId { get; set; }

        public int AgeAtRetirement { get; set; }
    
        public int Relationship { get; set; }
    
        public int Confession1 { get; set; }
    
        public int Confession2 { get; set; }

        public int NumberOfChildren { get; set; }
    
        public int Capital { get; set; }
    }
}
