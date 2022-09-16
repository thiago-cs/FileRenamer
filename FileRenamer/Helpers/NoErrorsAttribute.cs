using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace FileRenamer.Helpers;

/// <summary>
/// Specify that an INotifyDataErrorInfo data field must have no errors.
/// </summary>
public sealed class NoErrorsAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        return value is INotifyDataErrorInfo notify && notify.HasErrors
             ? new("This instance has errors.")
             : ValidationResult.Success;
    }
}