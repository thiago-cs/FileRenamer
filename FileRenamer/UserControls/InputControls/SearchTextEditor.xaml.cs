using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;


namespace FileRenamer.UserControls.InputControls;

[INotifyPropertyChanged]
public sealed partial class SearchTextEditor : UserControl
{
	#region Data

	[ObservableProperty]
	private SearchTextData _data;

	#endregion Data


	public SearchTextEditor()
	{
		InitializeComponent();
	}
}