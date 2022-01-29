#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;


namespace System.ComponentModel;

/// <summary>
/// A base class for validatable objects of which the properties must be observable.
/// </summary>
public abstract class BindableBase : INotifyPropertyChanging, INotifyPropertyChanged, INotifyDataErrorInfo, IValidatable
{
	#region INotifyPropertyChanging and INotifyPropertyChanged

	/// <inheritdoc cref="INotifyPropertyChanging.PropertyChanging" />
	public event PropertyChangingEventHandler? PropertyChanging;

	/// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	/// Compares the current and new values for a given property. If the value has changed,
	/// raises the <see cref="PropertyChanging" /> event, updates the property with the new
	/// value, then raises the <see cref="PropertyChanged" /> event.
	/// </summary>
	/// <typeparam name="T">The type of the property that changed.</typeparam>
	/// <param name="currentValue">The field storing the property's value.</param>
	/// <param name="newValue">The property's value after the change occurred.</param>
	/// <param name="propertyName">(optional) The name of the property that changed.</param>
	/// <returns><see langword="true" /> if the property was changed, <see langword="false" /> otherwise.</returns>
	/// <remarks>
	/// The <see cref="PropertyChanging" /> and <see cref="PropertyChanged" /> events are not raised
	/// if the current and new value for the target property are the same.
	/// </remarks>
	protected bool SetProperty<T>(ref T currentValue, T newValue, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
			return false;

		OnPropertyChanging(propertyName);

		currentValue = newValue;

		OnPropertyChanged(propertyName!, newValue);

		return true;
	}

	/// <summary>
	/// Raises the <see cref="PropertyChanging" /> event.
	/// </summary>
	/// <param name="propertyName">(optional) The name of the property that changed.</param>
	protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
	{
		PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
	}

	/// <summary>
	/// Raises the <see cref="PropertyChanged" /> event and validates the value of the member which changed.
	/// </summary>
	/// <param name="propertyName">(optional) The name of the property that changed.</param>
	/// <param name="value">The value to validate.</param>
	private void OnPropertyChanged(string propertyName, object? value)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		Validate(propertyName, value);
	}

	/// <summary>
	/// Raises the <see cref="PropertyChanged" /> event without validating the value of the member which changed.
	/// </summary>
	/// <param name="propertyName">(optional) The name of the property that changed.</param>
	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	#endregion


	#region INotifyDataErrorInfo

	/// <summary>
	/// The collection of member names and their respective validation results.
	/// </summary>
	private readonly Dictionary<string, List<ValidationResult>> dataErrors = new();


	/// <inheritdoc cref="INotifyDataErrorInfo.HasErrors" />
	public bool HasErrors => dataErrors.Any();

	/// <inheritdoc cref="INotifyDataErrorInfo.ErrorsChanged" />
	public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;


	/// <inheritdoc cref="INotifyDataErrorInfo.GetErrors(string?)" />
	public IEnumerable GetErrors(string? propertyName)
	{
		return GetErrorList(propertyName);
	}

	/// <summary>
	/// Gets a list of validation errors for a specified property or for the entire entity.
	/// </summary>
	/// <param name="propertyName">The name of the property to retrieve validation errors for. Or <see langword="null"/> or <see cref="string.Empty"/> to retrieve entity-level errors.</param>
	/// <returns>The validation errors for the property or entity.</returns>
	public List<ValidationResult> GetErrorList(string? propertyName)
	{
		// 1. Gets the errors for the specified property
		if (!string.IsNullOrEmpty(propertyName))
			return dataErrors[propertyName];

		// 2. Gets the errors for the entire entity.
		Dictionary<string, List<ValidationResult>>.ValueCollection values = dataErrors.Values;
		int count = 0;

		foreach (List<ValidationResult> value in values)
			count += value.Count;

		List<ValidationResult> list = new(count);

		foreach (List<ValidationResult> value in values)
			list.AddRange(value);

		return list;
	}

	/// <summary>
	/// Associates the specified errors with the specified member.
	/// </summary>
	/// <param name="propertyName">The name of the member with which the specified errors will be associated.</param>
	/// <param name="results">The error collection that should be added to the validation result collection already associated with the specified member.</param>
	private void AddErrors(string propertyName, List<ValidationResult> results)
	{
		if (dataErrors.TryGetValue(propertyName, out List<ValidationResult>? errors))
			errors.AddRange(results);
		else
			dataErrors.Add(propertyName, results);

		ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
	}

	/// <summary>
	/// Removes all validation results associated with the specified member.
	/// </summary>
	/// <param name="memberName">The name of the member of interest.</param>
	private void ClearErrors(string memberName)
	{
		if (dataErrors.TryGetValue(memberName, out List<ValidationResult>? errors))
		{
			errors.Clear();
			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(memberName));
		}
	}

	#endregion


	#region IValidatable

	/// <inheritdoc cref="IValidatable.Validate(string, object)" />
	public void Validate(string memberName, object? value)
	{
		ClearErrors(memberName);

		ValidationContext validationContext = new(this, null, null) { MemberName = memberName };
		List<ValidationResult> results = new();
		bool result = Validator.TryValidateProperty(value, validationContext, results);

		if (!result)
			AddErrors(memberName, results);
	}

	#endregion
}