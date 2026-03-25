namespace Tharga.Toolkit.Compare
{
    /// <summary>
    /// Defines a difference found during a deep comparison between two objects.
    /// </summary>
    public interface IDiff
    {
        /// <summary>
        /// Gets a description of the difference.
        /// </summary>
        string Message { get; }
        /// <summary>
        /// Gets the name or path of the first object in the comparison.
        /// </summary>
        string ObjectName { get; }
        /// <summary>
        /// Gets the name or path of the second object in the comparison.
        /// </summary>
        string OtherObjectName { get; }
        /// <summary>
        /// Gets the collection index where the difference was found, or null if not applicable.
        /// </summary>
        int? Index { get; }
    }
}