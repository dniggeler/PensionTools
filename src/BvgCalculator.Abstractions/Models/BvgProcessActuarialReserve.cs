using System;

namespace PensionCoach.Tools.BvgCalculator.Models
{
    public class BvgProcessActuarialReserve
    {
        /// <summary>
        /// Gets or sets the process date.
        /// </summary>
        /// <value>
        /// The process date.
        /// </value>
        public DateTime DateOfProcess { get; set; }

        /// <summary>
        /// Gets or sets the DKX.
        /// </summary>
        /// <value>
        /// The DKX.
        /// </value>
        public decimal Dkx { get; set; }

        /// <summary>
        /// Gets or sets the DKX1.
        /// </summary>
        /// <value>
        /// The DKX1.
        /// </value>
        public decimal Dkx1 { get; set; }

        /// <summary>
        /// Gets or sets the value before process.
        /// </summary>
        /// <value>
        /// The value before process.
        /// </value>
        public decimal ValueBeforeProcess { get; set; }

        /// <summary>
        /// Gets or sets the value after process.
        /// </summary>
        /// <value>
        /// The value after process.
        /// </value>
        public decimal ValueAfterProcess { get; set; }
    }
}
