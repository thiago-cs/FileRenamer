using Microsoft.UI.Xaml.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Models;


namespace FileRenamer.UserControls.Toolbar;

[ContentProperty(Name = nameof(Items))]
[INotifyPropertyChanged]
public sealed partial class ToolbarSplitButton
{
	[ObservableProperty]
	private ToolbarItemSize _itemSize;

	[ObservableProperty]
	private ExtendedUICommand _command;

	public System.Collections.Generic.ObservableVector<object> Items { get; } = new();

	public object FirstItem => Items.Count != 0 ? Items[0] : null;


	public ToolbarSplitButton()
	{
		InitializeComponent();
		DataContext = this;
	}
}