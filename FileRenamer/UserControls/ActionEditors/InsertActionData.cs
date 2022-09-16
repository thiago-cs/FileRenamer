using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.ValueSources;
using FileRenamer.Helpers;
using FileRenamer.ViewModels.ValueSources;
using FileRenamer.UserControls.InputControls;


namespace FileRenamer.UserControls.ActionEditors;

public sealed partial class InsertActionData : ObservableValidator
{
	#region Value Source Type

	public static readonly ValueSourceType[] ValueSourceTypes = System.Enum.GetValues<ValueSourceType>();

	[ObservableProperty]
	private ValueSourceType _valueSourceType;

	partial void OnValueSourceTypeChanged(ValueSourceType value)
	{
		ValueSourceViewModel = ToValueSourceViewModel(value);
	}

	#endregion

	#region Value Source ViewModel

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[NoErrors]
	private IValueSourceViewModel _valueSourceViewModel;

	partial void OnValueSourceViewModelChanging(IValueSourceViewModel value)
	{
		if (ValueSourceViewModel != null)
			ValueSourceViewModel.PropertyChanged -= ValueSourceViewModel_PropertyChanged;
	}

	partial void OnValueSourceViewModelChanged(IValueSourceViewModel value)
	{
		if (value != null)
			value.PropertyChanged += ValueSourceViewModel_PropertyChanged;
	}

	private void ValueSourceViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(HasErrors))
			ValidateProperty(ValueSourceViewModel, nameof(ValueSourceViewModel));
	}

	#endregion

	#region Index Data

	[NoErrors]
	public IndexEditorData IndexData { get; }

	private void IndexData_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(HasErrors))
			ValidateProperty(IndexData, nameof(IndexData));
	}

	#endregion

	public IValueSource ValueSource => ValueSourceViewModel?.ValueSource;


	#region Constructors

	public InsertActionData()
	{
		ValueSourceViewModel = new StringValueSourceViewModel();
		IndexData = new();

		Initialize();
	}

	public InsertActionData(InsertAction insertAction)
	{
		_valueSourceType = ToValueSourceType(insertAction.ValueSource);
		OnPropertyChanged(nameof(ValueSourceType));

		// Using {insertAction.ValueSource} instead of {_valueSourceType} is what allows us to edit existing data.
		ValueSourceViewModel = ToValueSourceViewModel(insertAction.ValueSource);

		IndexData = new(insertAction.InsertIndex);

		Initialize();
	}

	private void Initialize()
	{
		ValidateAllProperties();
		IndexData.PropertyChanged += IndexData_PropertyChanged;
	}

	#endregion


	#region Static helpers

	private static IValueSourceViewModel ToValueSourceViewModel(ValueSourceType type)
	{
		return type switch
		{
			ValueSourceType.FixedText => new StringValueSourceViewModel(),
			ValueSourceType.RandomText => new RandomStringValueSourceViewModel(),
			ValueSourceType.Counter => new CounterValueSourceViewModel(),
			_ => throw new System.NotImplementedException(),
		};
	}

	private static IValueSourceViewModel ToValueSourceViewModel(IValueSource valueSource)
	{
		return valueSource switch
		{
			StringValueSource vs => new StringValueSourceViewModel(vs),
			RandomStringValueSource vs => new RandomStringValueSourceViewModel(vs),
			CounterValueSource vs => new CounterValueSourceViewModel(vs),
			_ => throw new System.NotImplementedException(),
		};
	}

	private static ValueSourceType ToValueSourceType(IValueSource valueSource)
	{
		return valueSource switch
		{
			StringValueSource => ValueSourceType.FixedText,
			RandomStringValueSource => ValueSourceType.RandomText,
			CounterValueSource => ValueSourceType.Counter,
			_ => throw new System.NotImplementedException(),
		};
	}

	#endregion
}