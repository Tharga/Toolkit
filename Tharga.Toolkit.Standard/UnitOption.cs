namespace Tharga.Toolkit
{
    /// <summary>
    /// Represents the display options for a single time unit, including its value format and singular/plural labels.
    /// </summary>
    public class UnitOption
    {
        /// <summary>
        /// Gets or sets the format string used to display the numeric value for this unit.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets the label to display when the value is singular (e.g., "day").
        /// </summary>
        public string SignularSign { get; set; }
        /// <summary>
        /// Gets or sets the label to display when the value is plural (e.g., "days").
        /// </summary>
        public string PluralSign { get; set; }
    }
}