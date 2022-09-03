using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs.FileActions;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class ReplaceActionData : ObservableValidator
{
	#region Constants

	//public static readonly ExecutionScope[] executionScopeTypes = Enum.GetValues<ExecutionScope>();
	public static readonly ExecutionScope[] executionScopeTypes = { ExecutionScope.FileName, ExecutionScope.FileExtension, ExecutionScope.WholeInput, ExecutionScope.CustomRange };

	#endregion


	#region Execution Scope

	[ObservableProperty]
	private ExecutionScope _executionScope;

	partial void OnExecutionScopeChanged(ExecutionScope value)
	{
		ClearErrors(nameof(RangeData));

		if (ExecutionScope == ExecutionScope.CustomRange)
			ValidateProperty(RangeData, nameof(RangeData));
	}

	#endregion

	#region Texts

	[CustomValidation(typeof(ReplaceActionData), nameof(ValidateObservableValidator))]
	public SearchTextData OldString { get; } = new();

	public string NewString { get; set; }

	private void OldString_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(HasErrors))
			ValidateProperty(OldString, nameof(OldString));
	}

	#endregion

	#region Range

	[CustomValidation(typeof(ReplaceActionData), nameof(ValidateObservableValidator))]
	public TextRangeData RangeData { get; }

	private void RangeData_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(HasErrors))
			ValidateProperty(RangeData, nameof(RangeData));
	}

	#endregion


	#region Constructors

	public ReplaceActionData()
	{
		RangeData = new();
		ExecutionScope = ExecutionScope.FileName;

		Initialize();
	}

	public ReplaceActionData(ReplaceAction action)
	{
		OldString.TextType = action.UseRegex ? TextType.Regex : TextType.Text;
		OldString.IgnoreCase = action.IgnoreCase;
		OldString.Text = action.OldString;

		NewString = action.NewString;

		RangeData = new(action?.StartIndex, action?.EndIndex);
		ExecutionScope = GetScopeFromIndices(action?.StartIndex, action?.EndIndex);

		Initialize();
	}

	private void Initialize()
	{
		OldString.PropertyChanged += OldString_PropertyChanged;
		RangeData.PropertyChanged += RangeData_PropertyChanged;
		ValidateProperty(OldString, nameof(OldString));
	}

	#endregion


	#region Validation

	public static ValidationResult ValidateObservableValidator(INotifyDataErrorInfo notifier, ValidationContext _)
	{
		return notifier.HasErrors
			 ? new("This object has errors.")
			 : ValidationResult.Success;
	}

	#endregion


	public ReplaceAction GetRenameAction()
	{
		return ExecutionScope switch
		{
			ExecutionScope.WholeInput => new(OldString.Text, NewString, OldString.IgnoreCase, OldString.TextType == TextType.Regex),
			ExecutionScope.FileName => new(new BeginningIndex(), new FileExtensionIndex(), OldString.Text, NewString, OldString.IgnoreCase, OldString.TextType == TextType.Regex),
			ExecutionScope.FileExtension => new(new FileExtensionIndex(), new EndIndex(), OldString.Text, NewString, OldString.IgnoreCase, OldString.TextType == TextType.Regex),
			ExecutionScope.CustomRange => new(RangeData.StartIndexData.GetIIndex(), RangeData.EndIndexData.GetIIndex(), OldString.Text, NewString, OldString.IgnoreCase, OldString.TextType == TextType.Regex),
			_ => null,
		};
	}

	//public static string f(ValidationResult error) => error == null ? null : string.Join(",", error.MemberNames) + ": " + error.ErrorMessage;
	//public string[] MyErrors => GetErrors()?.Select(f).ToArray();
	//public string FirstError => f(GetErrors()?.FirstOrDefault());

	public static ExecutionScope GetScopeFromIndices(IIndex startIndex, IIndex endIndex)
	{
		return (startIndex, endIndex) switch
		{
			(BeginningIndex, FileExtensionIndex) => ExecutionScope.FileName,
			(FileExtensionIndex, EndIndex) => ExecutionScope.FileExtension,
			(BeginningIndex, EndIndex) => ExecutionScope.WholeInput,
			_ => ExecutionScope.CustomRange,
		};
	}
}