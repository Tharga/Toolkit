using System;

namespace Tharga.Toolkit
{
    /// <summary>
    /// Configuration options for formatting a duration, including the range of time units and string formatting options.
    /// </summary>
    public class DurationOptions
    {
        /// <summary>
        /// Gets or sets the base date and time value used as the reference point for duration calculation.
        /// </summary>
        public DateTime? BaseValue { get; set; }
        /// <summary>
        /// Gets or sets the largest time unit to include in the formatted duration. Defaults to <see cref="EUnit.Day"/>.
        /// </summary>
        public EUnit MaxUnit { get; set; } = EUnit.Day;
        /// <summary>
        /// Gets or sets the smallest time unit to include in the formatted duration. Defaults to <see cref="EUnit.Second"/>.
        /// </summary>
        public EUnit MinUnit { get; set; } = EUnit.Second;
        /// <summary>
        /// Gets or sets the string formatting options for the duration output.
        /// </summary>
        public DurationStringOptions StringOptions { get; set; }
    }
}