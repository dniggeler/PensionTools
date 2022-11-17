using System.Text.Json.Serialization;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;

namespace PensionCoach.Tools.EstvTaxCalculators.Models;

public class SimpleTaxRequest
{
    public int TaxYear { get; set; }

    [JsonPropertyName("TaxLocationID")]
    public int TaxLocationId { get; set; }
    public int Relationship { get; set; }
    public int Confession1 { get; set; }
    public IEnumerable<ChildModel> Children { get; set; }
    public int Confession2 { get; set; }
    public int TaxableIncomeCanton { get; set; }
    public int TaxableIncomeFed { get; set; }
    public int TaxableFortune { get; set; }
}
