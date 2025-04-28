namespace SOLTEC.PreBuildValidator.Exceptions;

/// <summary>
/// Represents an exception thrown when a validation fails in the pre-build validator.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationException"/> class with a specific message.
/// </remarks>
/// <param name="message">The message describing the validation error.</param>
public class ValidationException(string message) : Exception(message)
{
}
