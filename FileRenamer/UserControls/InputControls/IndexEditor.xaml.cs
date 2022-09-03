using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;


namespace FileRenamer.UserControls.InputControls;

[ObservableObject]
public sealed partial class IndexEditor
{
	#region Properties

	private bool _includePreposionInDescriptions;
	public bool IncludePreposionInDescriptions
	{
		get => _includePreposionInDescriptions;
		set
		{
			_includePreposionInDescriptions = value;
			IndexTypeSelector.DisplayMemberPath = value ? nameof(IndexTypeEntry.DescriptionWithPreposition) : nameof(IndexTypeEntry.Description);
		}
	}

	[ObservableProperty]
	private IndexEditorData _data;

	#endregion


	public IndexEditor()
	{
		InitializeComponent();

		IncludePreposionInDescriptions = true;
	}
}