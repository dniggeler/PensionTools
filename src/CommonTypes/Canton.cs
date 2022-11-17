using System.Text.Json.Serialization;

namespace PensionCoach.Tools.CommonTypes;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Canton
{
    /// <summary>
    /// Undefined is not a valid and is treated as an error
    /// </summary>
    Undefined = 0,

    AG = 1,

    AI = 2,

    AR = 3,
    
    /// <summary>
    /// Bern
    /// </summary>
    BE = 4,

    BL = 5,

    BS = 6,

    FR = 7,

    GE = 8,

    GL = 9,

    GR = 10,

    JU = 11,
    
    LU = 12,
    
    NE = 13,

    NW = 14,

    OW = 15,

    SH = 16,

    /// <summary>
    /// Solothurn
    /// </summary>
    SO = 17,

    /// <summary>
    /// St.Gallen
    /// </summary>
    SG = 18,
    
    SZ = 19,

    TI = 20,

    TG = 21,

    UR = 22,

    VD = 23,

    VS = 24,

    ZG = 25,

    /// <summary>
    /// Zürich
    /// </summary>
    ZH = 26,
}
