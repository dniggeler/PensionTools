using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReligiousGroupType
    {
        /// <summary>
        /// Undefined or none of the following
        /// </summary>
        Other=0,

        /// <summary>
        /// reformiert in german
        /// </summary>
        Protestant =1,

        /// <summary>
        /// katholisch in german
        /// </summary>
        Catholic =2,

        /// <summary>
        /// römisch-katholisch in german
        /// </summary>
        Roman =3
    }
}