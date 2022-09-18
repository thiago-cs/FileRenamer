using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FileRenamer.Core.Jobs;


namespace FileRenamer.Views.Components;

public sealed partial class FolderView : UserControl
{
	private IList<JobTarget> _items;
	public IList<JobTarget> Items
	{
		get => _items;
		set
		{
			_items = value;
			ItemsControl.ItemsSource = _items;
		}
	}

	public event EventHandler TestRequested;


	public FolderView()
	{
		InitializeComponent();
	}


	private void TestNameMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
	{
		var menuItem = sender as MenuFlyoutItem;
		TestRequested?.Invoke(menuItem.DataContext, EventArgs.Empty);
	}
}