using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs;


namespace FileRenamer.UserControls.InputControls;

[ObservableObject]
public sealed partial class JobScopeSelector : UserControl
{
	#region Scopes DependencyProperty
	/// <summary>
	/// Gets or sets the available job scopes to chose from.
	/// </summary>
	public JobScopes Scopes
	{
		get => (JobScopes)GetValue(ScopesProperty);
		set => SetValue(ScopesProperty, value);
	}

	// Using a DependencyProperty as the backing store for Scope.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty ScopesProperty =
		DependencyProperty.Register(
			nameof(Scopes),
			typeof(JobScopes),
			typeof(JobScopeSelector),
			new PropertyMetadata(JobScopes.Files, OnScopeChanged));

	private static void OnScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is JobScopeSelector This && e.NewValue is JobScopes value)
		{
			This.FolderToggle.IsChecked = value.HasFlag(JobScopes.Folders);
			This.FileToggle.IsChecked = value.HasFlag(JobScopes.Files);
		}
	}
	#endregion Scope DependencyProperty

	private void UpdateScopes()
	{
		JobScopes scopes = JobScopes.None;

		if (FolderToggle.IsChecked == true)
			scopes |= JobScopes.Folders;

		if (FileToggle.IsChecked == true)
			scopes |= JobScopes.Files;

		Scopes = scopes;
	}


	public JobScopeSelector()
	{
		InitializeComponent();
		Scopes= JobScopes.None;
	}


	private void ToggleButton_Checked(object sender, RoutedEventArgs e)
	{
		UpdateScopes();
	}

	private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
	{
		UpdateScopes();
	}
}