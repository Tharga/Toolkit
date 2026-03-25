using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tharga.Toolkit
{
    /// <summary>
    /// Base class for creating enumeration types that behave like strongly-typed enum alternatives with rich behavior.
    /// </summary>
    public abstract class Enumeration : IComparable
    {
        /// <summary>
        /// Gets the numeric identifier for this enumeration value.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Gets the display name for this enumeration value.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumeration"/> class.
        /// </summary>
        /// <param name="id">The numeric identifier.</param>
        /// <param name="name">The display name.</param>
        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Returns the display name of this enumeration value.
        /// </summary>
        /// <returns>The <see cref="Name"/> of this enumeration value.</returns>
        public override string ToString() => Name;

        /// <summary>
        /// Gets all declared enumeration values of the specified type.
        /// </summary>
        /// <typeparam name="T">The enumeration type to retrieve values for.</typeparam>
        /// <returns>An enumerable of all public static fields of the specified enumeration type.</returns>
        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        /// <summary>
        /// Determines whether the specified object is equal to this enumeration value by comparing type and <see cref="Id"/>.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns>True if the objects are the same type and have the same <see cref="Id"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().AssemblyQualifiedName == obj.GetType().AssemblyQualifiedName;
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        /// <summary>
        /// Returns a hash code for this enumeration value.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Compares this enumeration value to another by their <see cref="Id"/>.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>A value indicating the relative order of the objects being compared.</returns>
        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    }
}