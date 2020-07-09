using System;

namespace PensionCoach.Tools.BvgCalculator.Models
{
    public class BvgRetirementCapital
    {
        /// <summary>
        /// Date of validity
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Mandatory retirement assets BVG portion
        /// </summary>
        public decimal Value { get; }


        /// <summary>
        /// Gets the value without interest BVG by raw retirement credits.
        /// </summary>
        /// <value>
        /// The value without interest BVG by raw retirement credits.
        /// </value>
        public decimal ValueWithoutInterest { get; }

        public BvgRetirementCapital(DateTime date, decimal value, decimal valueWithoutInterest)
        {
            Date = date;
            Value = value;
            ValueWithoutInterest = valueWithoutInterest;
        }
    }
}