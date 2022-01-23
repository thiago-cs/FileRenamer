using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using Windows.Foundation.Collections;


namespace FileRenamer.UserControls;

[Microsoft.UI.Xaml.Markup.ContentProperty(Name = nameof(Items))]
public sealed partial class ToolbarGroup
{
	private const int maxRows = 3;


	#region Header DependencyProperty
	public string Header
	{
		get => (string)GetValue(HeaderProperty);
		set => SetValue(HeaderProperty, value);
	}

	public static readonly DependencyProperty HeaderProperty =
		DependencyProperty.Register(
			nameof(Header),
			typeof(string),
			typeof(ToolbarGroup),
			new PropertyMetadata(""));
	#endregion Header DependencyProperty

	public ObservableVector<FrameworkElement> Items { get; } = new();


	public ToolbarGroup()
	{
		InitializeComponent();

		Items.VectorChanged += Items_VectorChanged;
	}


	private void Items_VectorChanged(IObservableVector<FrameworkElement> sender, IVectorChangedEventArgs e)
	{
		// 1. 
		int columns = (Items.Count + maxRows - 1) / maxRows;

		while (MyPanel.ColumnDefinitions.Count < columns)
			MyPanel.ColumnDefinitions.Add(new() { Width = GridLength.Auto });

		// 2. 
		int index = (int)e.Index;
		UIElementCollection children = MyPanel.Children;

		switch (e.CollectionChange)
		{
			case CollectionChange.Reset:
				children.Clear();
				break;

			case CollectionChange.ItemInserted:
				children.Add(Items[index]);
				break;

			case CollectionChange.ItemRemoved:
				children.RemoveAt(index);
				break;

			case CollectionChange.ItemChanged:
				{
					FrameworkElement oldElement = children[index] as FrameworkElement;
					int col = Grid.GetColumn(oldElement);
					int row = Grid.GetRow(oldElement);

					FrameworkElement newElement = Items[index];
					Grid.SetColumn(newElement, col);
					Grid.SetRow(newElement, row);

					children[index] = newElement;
				}
				return;

			default:
				break;
		}

		// 3. 
		IList<UIElement> list = children;

		for (int i = index; i < list.Count; i++)
		{
			FrameworkElement element = list[i] as FrameworkElement;
			int col = i / maxRows;
			int row = i % maxRows;

			Grid.SetColumn(element, col);
			Grid.SetRow(element, row);
		}
	}
}