using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs;


namespace FileRenamer.Views.Components;

public sealed partial class JobScopeSelector : UserControl
{
	#region Scope DependencyProperty
	public JobScope Scope
	{
		get => (JobScope)GetValue(ScopeProperty);
		set => SetValue(ScopeProperty, value);
	}

	// Using a DependencyProperty as the backing store for Scope.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty ScopeProperty =
		DependencyProperty.Register(
			nameof(Scope),
			typeof(JobScope),
			typeof(JobScopeSelector),
			new PropertyMetadata(JobScope.Files, OnScopeChanged));

	private static void OnScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is JobScopeSelector jobScopeSelector)
			jobScopeSelector.Radios.SelectedIndex = (int)e.NewValue;
	}
	#endregion Scope DependencyProperty


	public JobScopeSelector()
	{
		InitializeComponent();
	}


	private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		int selectedIndex = Radios.SelectedIndex;

		if (selectedIndex != -1)
			Scope = (JobScope)selectedIndex;
	}
}