using System;
using PensionCoach.Tools.CommonTypes;

namespace PensionCoach.Tools.BvgCalculator.Models
{
    public class BvgPerson
    {
        /// <summary>
        ///     Gets or sets the birthdate.
        /// </summary>
        /// <value>
        ///     The birthdate.
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the gender code.
        /// </summary>
        /// <value>
        /// The gender code.
        /// </value>
        public Gender Gender { get; set; }

        /// <summary>
        ///     Gets or sets the part time degree.
        /// </summary>
        /// <value>
        ///     The part time degree.
        /// </value>
        /// <remarks>
        ///     TZGR
        /// </remarks>
        public decimal PartTimeDegree { get; set; }

        /// <summary>
        ///     Gets or sets the disability degree BVG.
        /// </summary>
        /// <value>
        ///     The disability degree BVG.
        /// </value>
        public decimal DisabilityDegree { get; set; }

        /// <summary>
        /// Gets or sets the reported salary.
        /// </summary>
        /// <value>
        /// The reported salary.
        /// </value>
        public decimal ReportedSalary { get; set; }
    }
}
