namespace PensionCoach.Tools.CommonTypes.MultiPeriod
{
    /// <summary>
    /// Simulates the change of the residence. A change always happens at the end of a period.
    /// </summary>
    public record ChangeResidenceAction
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; init; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; init; }

        /// <summary>
        /// Gets or sets the ordinal to define a linear order between multiple cash-flows.
        /// </summary>
        /// <value>
        /// The ordinal.
        /// </value>
        public int Ordinal { get; init; }

        /// <summary>
        /// Gets the change of residence at this year.
        /// </summary>
        /// <value>
        /// The change of residence at this year.
        /// </value>
        public int ChangeAtYear { get; set; }

        /// <summary>
        /// Gets the destination municipality identifier.
        /// </summary>
        /// <value>
        /// The destination municipality identifier.
        /// </value>
        public int DestinationMunicipalityId { get; set; }

        /// <summary>
        /// Gets or sets the destination canton.
        /// </summary>
        /// <value>
        /// The destination canton.
        /// </value>
        public Canton DestinationCanton { get; set; }

        /// <summary>
        /// Gets the change cost. The cost to change of the residence is deducted from wealth.
        /// </summary>
        /// <value>
        /// The change cost.
        /// </value>
        public decimal ChangeCost { get; set; }
    }
}
