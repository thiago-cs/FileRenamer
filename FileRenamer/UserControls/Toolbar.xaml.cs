namespace FileRenamer.UserControls;

[Microsoft.UI.Xaml.Markup.ContentProperty(Name = nameof(Groups))]
public sealed partial class Toolbar
{
	public System.Collections.Generic.ObservableVector<ToolbarGroup> Groups { get; } = new();


	public Toolbar()
	{
		InitializeComponent();
	}
}