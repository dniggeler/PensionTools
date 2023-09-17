using Domain.Enums;

namespace Domain.Models.Municipality;

public class MunicipalitySearchFilter
{
    /// <summary>
    /// Gets or sets the canton defined by enum.
    /// If undefined it is not active in search.
    /// </summary>
    /// <value>
    /// The canton.
    /// </value>
    public Canton Canton { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// Municipality name is searched by name (sub-string).
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the year of validity.
    /// Municipalities have a mutation history.
    /// Only records younger or equal to validity year are matched.
    /// </summary>
    /// <value>
    /// The year of validity.
    /// </value>
    public int? YearOfValidity { get; set; }
}
