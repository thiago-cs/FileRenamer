using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core;
using FileRenamer.Core.Jobs;
using FileRenamer.Helpers;


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


	public TextPreviewer()
	{
		System.Action action = ProcessInput;
		debouncedProcessInputAsync = action.DebounceAsync(debounceInterval);
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
}