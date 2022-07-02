using System.Text.Json.Serialization;

namespace PensionCoach.Tools.PostOpenApi.Models;

public class OpenApiZipFields
{
    [JsonPropertyName("bfsnr")]
    public int BfsCode { get; set; }

    [JsonPropertyName("postleitzahl")]
    public string ZipCode { get; set; }

    [JsonPropertyName("plz_zz")]
    public string ZipCodeAddOn { get; set; }

    [JsonPropertyName("gilt_ab_dat")]
    public string DateOfValidity { get; set; }

    [JsonPropertyName("ortbez27")]
    public string MunicipalityName { get; set; }

    [JsonPropertyName("kanton")]
    public string Canton { get; set; }

    [JsonPropertyName("sprachcode")]
    public int LanguageCode { get; set; }
}
