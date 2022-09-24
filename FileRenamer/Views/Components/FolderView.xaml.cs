using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.FileSystem;


namespace FileRenamer.Views.Components;

[INotifyPropertyChanged]
public sealed partial class FolderView : UserControl
{
	private List<IItem> itemsInFolder;

	#region Project property

	[ObservableProperty]
	private Project _project;

	partial void OnProjectChanging(Project value)
	{
		if (value != null)
		{
			value.PropertyChanged -= Project_PropertyChanged;
			value.Jobs.CollectionChanged -= Jobs_CollectionChanged;
			value.Jobs.NestedJobChanged -= Jobs_NestedJobChanged;
		}
	}

	partial void OnProjectChanged(Project value)
	{
		if (value != null)
		{
			value.PropertyChanged += Project_PropertyChanged;
			value.Jobs.CollectionChanged += Jobs_CollectionChanged;
			value.Jobs.NestedJobChanged += Jobs_NestedJobChanged;
		}
	}

	private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Project.Scope))
			UpdatePreview();
	}

	private void Jobs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		UpdatePreview();
	}

	private void Jobs_NestedJobChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		UpdatePreview();
	}

	#endregion

	#region Folder property

	/// <summary>
	/// Gets or sets the current working directory on which file operations are run.
	/// </summary>
	[ObservableProperty]
	private IFolder _folder;

	partial void OnFolderChanged(IFolder value)
	{
		if (value != null)
			_ = UpdateItemsInFolderAsync();
	}

	private async Task UpdateItemsInFolderAsync()
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

		UpdatePreview();
	}

	#endregion

	#region Items property

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

	#endregion

	public event EventHandler TestRequested;


	public FolderView()
	{
		InitializeComponent();
	}


	private void UpdatePreview()
	{
		if (PART_Expander.IsExpanded)
			ItemsControl.ItemsSource = itemsInFolder != null
					? (IList<JobTarget>)Project.ComputeChanges(itemsInFolder)
					: null;
	}

	private void Expander_Expanding(Expander sender, ExpanderExpandingEventArgs args)
	{
		UpdatePreview();
	}

	private void TestNameMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
	{
		var menuItem = sender as MenuFlyoutItem;
		TestRequested?.Invoke(menuItem.DataContext, EventArgs.Empty);
	}
}