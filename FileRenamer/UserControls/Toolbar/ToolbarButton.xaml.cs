using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Models;


namespace FileRenamer.UserControls.Toolbar;

[INotifyPropertyChanged]
public sealed partial class ToolbarButton
{
	#region Properties

	[ObservableProperty]
	private ToolbarItemSize _itemSize;

	[ObservableProperty]
	private ExtendedUICommand _command;

	#endregion


	#region Constructors

	public ToolbarButton()
	{
		InitializeComponent();
	}

	#endregion
}