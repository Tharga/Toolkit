namespace Tharga.Toolkit
{
    /// <summary>
    /// Represents the types of validation errors that can occur.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>No value was provided.</summary>
        NoValue,
        /// <summary>The value has an invalid format.</summary>
        InvalidFormat,
        /// <summary>The check digit is invalid.</summary>
        InvalidCheckDigit
    }
}