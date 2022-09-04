using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Models;


namespace FileRenamer.UserControls.Toolbar;

[INotifyPropertyChanged]
public sealed partial class ToolbarButton : UserControl
{
	#region Properties

	[ObservableProperty]
	private ToolbarItemSize _itemSize;

	[ObservableProperty]
	private UICommandBase _command;

	partial void OnCommandChanged(UICommandBase value)
	{
		if (value.KeyboardAccelerator != null)
			KeyboardAccelerators.Add(value.KeyboardAccelerator);
	}

	#endregion


	#region Constructors

	public ToolbarButton()
	{
		InitializeComponent();
	}

	#endregion
}