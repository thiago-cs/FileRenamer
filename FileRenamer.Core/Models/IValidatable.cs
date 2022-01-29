namespace System.ComponentModel;

public interface IValidatable
{
	/// <summary>
	/// Validates the specified value for the member with the specified name according to its associated <see cref="DataAnnotations.ValidationAttribute"/> attributes.
	/// </summary>
	/// <param name="memberName">The name of the member to validate.</param>
	/// <param name="value">The value to validate.</param>
	void Validate(string memberName, object value);
}