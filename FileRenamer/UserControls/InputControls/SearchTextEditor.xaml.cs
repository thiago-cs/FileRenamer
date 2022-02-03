using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace FileRenamer.UserControls.InputControls;

public sealed partial class SearchTextEditor : UserControl
{
	#region Data DependencyProperty
	public SearchTextData Data
	{
		get => (SearchTextData)GetValue(DataProperty);
		set => SetValue(DataProperty, value);
	}

	// Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty DataProperty =
		DependencyProperty.Register(
			nameof(Data),
			typeof(SearchTextData),
			typeof(SearchTextEditor),
			new PropertyMetadata(null));
	#endregion Data DependencyProperty


	public SearchTextEditor()
	{
		InitializeComponent();
	}
}