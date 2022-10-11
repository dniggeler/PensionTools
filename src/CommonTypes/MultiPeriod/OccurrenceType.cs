using System.Text.Json.Serialization;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OccurrenceType
{
    BeginOfPeriod,
    EndOfPeriod,
}
