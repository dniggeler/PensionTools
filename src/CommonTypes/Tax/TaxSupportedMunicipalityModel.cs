namespace PensionCoach.Tools.CommonTypes.Tax
{
    public record TaxSupportedMunicipalityModel
    {
        public int BfsMunicipalityNumber { get; set; }

        public string Name { get; set; }

        public Canton Canton { get; set; }

        /// <summary>
        /// Gets or sets the maximum supported year. Tax data for cantons or municipalities may not be on equally
        /// available for a given year. Thus, it indicates the most current year for which tax data are available.
        /// </summary>
        /// <value>
        /// The maximum supported year.
        /// </value>
        public int MaxSupportedYear { get; set; }
    }
}
