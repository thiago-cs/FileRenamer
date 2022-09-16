using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Helpers;
using FileRenamer.UserControls.InputControls;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class MoveStringActionData : ObservableValidator
{
	private const string ZeroCountErrorMessage = "Enter a value other than zero.";


	#region Texts

	[NoErrors]
	public SearchTextData OldString { get; } = new();

	private void OldString_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(HasErrors))
			ValidateProperty(OldString, nameof(OldString));
	}

	#endregion

	#region Count

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[CustomValidation(typeof(MoveStringActionData), nameof(ValidateCount))]
	private int _count;

	[ObservableProperty]
	private string _countErrorMessage;

	#endregion


	#region Constructors

	public MoveStringActionData()
	{
		Initialize();
	}

	public MoveStringActionData(MoveStringAction action)
	{
		OldString.TextType = action.UseRegex ? TextType.Regex : TextType.Text;
		OldString.IgnoreCase = action.IgnoreCase;
		OldString.Text = action.Text;

		Count = action.Count;

		Initialize();
	}

	private void Initialize()
	{
		OldString.PropertyChanged += OldString_PropertyChanged;
		ValidateAllProperties();
	}

	#endregion


	#region Validation

	public static ValidationResult ValidateCount(int value, ValidationContext context)
	{
		MoveStringActionData data = context.ObjectInstance as MoveStringActionData;

		if (value == 0)
		{
			data.CountErrorMessage = ZeroCountErrorMessage;
			return new(ZeroCountErrorMessage);
		}
		else
		{
			data.CountErrorMessage = null;
			return ValidationResult.Success;
		}
	}

	#endregion


	public MoveStringAction GetRenameAction()
	{
		return new(OldString.Text, OldString.IgnoreCase, OldString.TextType == TextType.Regex, Count);
	}
}