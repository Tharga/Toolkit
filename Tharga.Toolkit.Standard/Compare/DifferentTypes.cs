using System;

namespace Tharga.Toolkit.Compare
{
    /// <summary>
    /// Represents a difference where two compared objects have different types.
    /// </summary>
    public class DifferentTypes : IDiff
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DifferentTypes"/> class.
        /// </summary>
        /// <param name="objectName">The name or path of the object where the type mismatch was found.</param>
        /// <param name="type">The type of the first object.</param>
        /// <param name="otherType">The type of the second object.</param>
        /// <param name="index">The collection index where the difference was found, or null if not applicable.</param>
        public DifferentTypes(string objectName, Type type, Type otherType, int? index)
        {
            ObjectName = objectName;
            Message = string.Format("The types differs. One type is {0} and the other is {1} in object {2}.", type, otherType, objectName);
            Index = index;
        }

        /// <summary>
        /// Gets a description of the type difference.
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// Gets the name or path of the object where the type mismatch was found.
        /// </summary>
        public string ObjectName { get; }
        /// <summary>
        /// Gets the name of the other object. Always returns an empty string for type differences.
        /// </summary>
        public string OtherObjectName => string.Empty;
        /// <summary>
        /// Gets the collection index where the difference was found, or null if not applicable.
        /// </summary>
        public int? Index { get; }
    }
}