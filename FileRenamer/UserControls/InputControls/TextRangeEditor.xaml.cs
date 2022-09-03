using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;


namespace FileRenamer.UserControls.InputControls;

[ObservableObject]
public sealed partial class TextRangeEditor : UserControl
{
	[ObservableProperty]
	private TextRangeData _data;


	public TextRangeEditor()
	{
		InitializeComponent();
	}
}