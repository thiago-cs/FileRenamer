using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.Models;


namespace FileRenamer.UserControls;

public sealed partial class ToolbarButton : UserControl
{
	#region Fields

	private const string mediumToolbarButtonStyleKey = "ToolbarItemSize.Medium";
	private static Style mediumToolbarButtonStyle;

	#endregion


	#region ItemSize DependencyProperty
	public ToolbarItemSize ItemSize
	{
		get => (ToolbarItemSize)GetValue(ItemSizeProperty);
		set => SetValue(ItemSizeProperty, value);
	}

	public static readonly DependencyProperty ItemSizeProperty =
		DependencyProperty.Register(
			nameof(ItemSize),
			typeof(ToolbarItemSize),
			typeof(MainWindow),
			new PropertyMetadata(ToolbarItemSize.Medium, OnItemSizeChanged));

	private static void OnItemSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is ToolbarButton button)
			button.MyContentControl.Style = (ToolbarItemSize)e.NewValue switch
			{
				ToolbarItemSize.Medium or _ => mediumToolbarButtonStyle,
			};
	}
	#endregion Size DependencyProperty

	#region Command DependencyProperty
	public UICommand Command
	{
		get => (UICommand)GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	public static readonly DependencyProperty CommandProperty =
		DependencyProperty.Register(
			nameof(Command),
			typeof(UICommand),
			typeof(ToolbarButton),
			new PropertyMetadata(null, OnCommandChanged));

	private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is ToolbarButton button)
			button.MyContentControl.DataContext = e.NewValue;
	}
	#endregion Command DependencyProperty


	#region Constructors

	public ToolbarButton()
	{
		InitializeComponent();

		if (mediumToolbarButtonStyle == null)
			if (Resources.TryGetValue(mediumToolbarButtonStyleKey, out object o))
				mediumToolbarButtonStyle = o as Style;

		MyContentControl.Style = mediumToolbarButtonStyle;
	}

	#endregion
}