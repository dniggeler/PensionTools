using System.Text.Json.Serialization;

namespace PensionCoach.Tools.CommonTypes
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CivilStatus
    {
        /// <summary>
        /// Undefined is not a valid and is treated as an error
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Single means not married, ie. widowed and divorced persons are treated as singles
        /// </summary>
        Single = 1,

        /// <summary>
        /// A married person
        /// </summary>
        Married = 2
    }
}
