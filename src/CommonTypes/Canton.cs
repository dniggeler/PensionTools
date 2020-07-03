using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PensionCoach.Tools.CommonTypes
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Canton
    {
        /// <summary>
        /// Undefined is not a valid and is treated as an error
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Zürich
        /// </summary>
        ZH = 1,

        /// <summary>
        /// Bern
        /// </summary>
        BE = 2,

        /// <summary>
        /// Solothurn
        /// </summary>
        SO = 3,

        /// <summary>
        /// St.Gallen
        /// </summary>
        SG = 4,

        AG = 5,

        GE = 6,

        JU = 7,

        ZG = 8,

        GR = 9,

        VD = 10,

        VS = 11,

        TI = 12,

        LU = 13,

        UR = 14,

        BS = 15,

        BL = 16,

        FR = 17,

        NE = 18,

        SH = 19,

        TG = 20,

        SZ = 21,

        GL = 22,

        OW = 23,

        NW = 24,

        AR = 25,

        AI = 26,
    }
}