using System.Text.Json.Serialization;

namespace PensionCoach.Tools.PostOpenApi.Models;

public class OpenApiZipRecord
{
    [JsonPropertyName("record")]
    public OpenApiZipDetail Record { get; set; }
}
