using System.Text.Json.Serialization;

namespace Domain.Enums
{
    /// <summary>
    /// Represents the gender of a human being.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        /// <summary>
        /// The gender is not defined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The gender is female.
        /// </summary>
        Female = 1,

        /// <summary>
        /// The gender male.
        /// </summary>
        Male = 2
    }
}
