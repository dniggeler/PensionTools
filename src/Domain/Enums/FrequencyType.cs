using System.Text.Json.Serialization;

namespace PensionCoach.Tools.CommonTypes;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FrequencyType
{
    Daily,
    Monthly,
    Yearly
}
