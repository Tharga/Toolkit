namespace Tharga.Toolkit.Compare
{
    /// <summary>
    /// Represents a difference found during a deep comparison between two objects.
    /// </summary>
    public class Diff : IDiff
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Diff"/> class.
        /// </summary>
        /// <param name="objectName">The name or path of the first object.</param>
        /// <param name="otherObjectName">The name or path of the second object.</param>
        /// <param name="message">A description of the difference.</param>
        /// <param name="index">The collection index where the difference was found, or null if not applicable.</param>
        public Diff(string objectName, string otherObjectName, string message, int? index)
        {
            ObjectName = objectName ?? "N/A";
            OtherObjectName = otherObjectName ?? "N/A";
            Message = message;
            Index = index;

            if (ObjectName.Contains("[]") && index != null)
            {
                ObjectName = ObjectName.Replace("[]", $"[{index}]");
            }
        }

        /// <summary>
        /// Gets the name or path of the first object in the comparison.
        /// </summary>
        public string ObjectName { get; }
        /// <summary>
        /// Gets the collection index where the difference was found, or null if not applicable.
        /// </summary>
        public int? Index { get; }
        /// <summary>
        /// Gets the name or path of the second object in the comparison.
        /// </summary>
        public string OtherObjectName { get; }
        /// <summary>
        /// Gets a description of the difference.
        /// </summary>
        public string Message { get; }
    }
}