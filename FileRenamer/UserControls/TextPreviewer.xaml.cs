using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core;
using FileRenamer.Core.Jobs;
using FileRenamer.Helpers;
using FileRenamer.Models;


namespace FileRenamer.UserControls;

[ObservableObject]
public sealed partial class TextPreviewer : UserControl
{
	private readonly System.Func<Task> debouncedProcessInputAsync;
	private const int debounceInterval = 400; // milliseconds


	[ObservableProperty]
	private Project _project;

	partial void OnProjectChanging(Project value)
	{
		if (_project == null)
			return;

		_project.Jobs.CollectionChanged -= Jobs_CollectionChanged;
		_project.Jobs.NestedJobChanged -= Jobs_NestedJobChanged;
	}

	partial void OnProjectChanged(Project value)
	{
		if (_project == null)
			return;

		ProcessInput();

		_project.Jobs.CollectionChanged += Jobs_CollectionChanged;
		_project.Jobs.NestedJobChanged += Jobs_NestedJobChanged;
	}

	private string _input;
	public string Input
	{
		get => _input;
		set
		{
			if (SetProperty(ref _input, value))
				_ = debouncedProcessInputAsync();
		}
	}

	private string _output;
	public string Output { get => _output; private set => SetProperty(ref _output, value); }

	[ObservableProperty]
	private Visibility _copyOutputButtonVisibility = Visibility.Collapsed;

	public ExtendedUICommand CopyOutputCommand { get; private init; }


	public TextPreviewer()
	{
		System.Action action = ProcessInput;
		debouncedProcessInputAsync = action.DebounceAsync(debounceInterval);

		CopyOutputCommand = new()
		{
			Description = "Copy this output to the clipboard",
			Label = "Copy",
			IconSource = ExtendedUICommand.CreateIconSource(Symbol.Copy),
		};
		CopyOutputCommand.ExecuteRequested += CopyCommand_ExecuteRequested;


		InitializeComponent();
	}


	private void ProcessInput()
	{
		if (string.IsNullOrEmpty(Input) || Project == null)
		{
			Output = string.Empty;
			return;
		}

		Project.Jobs.ResetState();

		JobTarget target = new(new Core.FileSystem.FileMock(Input), 0);
		JobContext context = new(Project.Jobs, new[] { target });

		Project.Jobs.Run(target, context);
		Output = target.NewFileName;
	}

	public void ProcessName(string name)
	{
		Input = name;
		ProcessInput();
	}


	private void Jobs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		ProcessInput();
	}

	private void Jobs_NestedJobChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		ProcessInput();
	}

	private void OutputPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
	{
		CopyOutputButtonVisibility = Visibility.Visible;
	}

	private void OutputPanel_PointerExited(object sender, PointerRoutedEventArgs e)
	{
		CopyOutputButtonVisibility = Visibility.Collapsed;
	}

	private void CopyCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
	{
		if (string.IsNullOrWhiteSpace(Output))
			return;

		DataPackage dataPackage = new() { RequestedOperation = DataPackageOperation.Copy };
		dataPackage.SetText(Output);
		Clipboard.SetContent(dataPackage);
	}
}