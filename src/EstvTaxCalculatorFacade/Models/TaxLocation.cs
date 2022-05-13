using System.Text.Json.Serialization;

namespace PensionCoach.Tools.EstvTaxCalculators.Models;

public class TaxLocation
{
    [JsonPropertyName("TaxLocationID")]
    public int Id { get; set; }

    public string ZipCode { get; set; }

    [JsonPropertyName("BfsID")]
    public int BfsId { get; set; }

    [JsonPropertyName("CantonID")]
    public int CantonId { get; set; }
    
    public string BfsName { get; set; }
    
    public string City { get; set; }
    
    public string Canton { get; set; }
}
