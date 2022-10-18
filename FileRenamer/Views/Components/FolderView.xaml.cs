using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.FileSystem;


namespace FileRenamer.Views.Components;

[INotifyPropertyChanged]
public sealed partial class FolderView : UserControl
{
	#region Project property

	[ObservableProperty]
	private Project _project;

	partial void OnProjectChanging(Project value)
	{
		if (Project == null)
			return;

		Project.PropertyChanged -= Project_PropertyChanged;
		Project.Jobs.CollectionChanged -= Jobs_CollectionChanged;
		Project.Jobs.NestedJobChanged -= Jobs_NestedJobChanged;
	}

	partial void OnProjectChanged(Project value)
	{
		if (Project == null)
			return;

		Project.PropertyChanged += Project_PropertyChanged;
		Project.Jobs.CollectionChanged += Jobs_CollectionChanged;
		Project.Jobs.NestedJobChanged += Jobs_NestedJobChanged;

		RefreshView();
	}

	private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Project.Scope))
			RefreshView();
	}

	private void Jobs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		RefreshView();
	}

	private void Jobs_NestedJobChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		RefreshView();
	}

	#endregion

	#region Folder property

	/// <summary>
	/// Gets or sets the current working directory on which file operations are run.
	/// </summary>
	[ObservableProperty]
	private IFolder _folder;

	private List<IItem> itemsInFolder;

	partial void OnFolderChanged(IFolder value)
	{
		if (value != null)
			_ = RefreshItemsInFolderAsync();

		RefreshItemsInFolderCommand.NotifyCanExecuteChanged();
	}

	#endregion

	#region Refresh items in folder command

	[RelayCommand(CanExecute = nameof(CanRefreshItemsInFolder))]
	private async Task RefreshItemsInFolderAsync()
	{
		if (Folder == null)
		{
			itemsInFolder = null;
		}
		else
		{
			itemsInFolder = new();
			itemsInFolder.AddRange(await Folder.GetSubfoldersAsync());
			itemsInFolder.AddRange(await Folder.GetFilesAsync());
		}

		RefreshView();
	}

	private bool CanRefreshItemsInFolder()
	{
		return Folder != null && PART_Expander.IsExpanded;
	}

	#endregion

	#region IsExpanded property

	[ObservableProperty]
	private bool _isExpanded = true;

	partial void OnIsExpandedChanged(bool value)
	{
		RefreshItemsInFolderCommand.NotifyCanExecuteChanged();

		if (value) // if expanding...
			RefreshView();
	}

	#endregion

	#region Items property

	private IList<JobTarget> _items;

	public IList<JobTarget> Items
	{
		get => _items;
		private set
		{
			_items = value;

			if (ItemsControl != null)
				ItemsControl.ItemsSource = _items;
		}
	}

	#endregion

	public event EventHandler TestRequested;


	public FolderView()
	{
		InitializeComponent();
	}


	private void RefreshView()
	{
		if (PART_Expander.IsExpanded)
			Items = itemsInFolder != null
				  ? Project?.ComputeChanges(itemsInFolder)
				  : null;
	}


	private void TestNameMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
	{
		var menuItem = sender as MenuFlyoutItem;
		TestRequested?.Invoke(menuItem.DataContext, EventArgs.Empty);
	}

	private void StackPanel_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
	{
		e.Handled = true;
	}
}