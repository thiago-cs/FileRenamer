using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.Conditionals;
using FileRenamer.Helpers;
using FileRenamer.UserControls.ActionEditors;
using FileRenamer.UserControls.InputControls;


namespace FileRenamer.UserControls.ConditionalJobEditors;

public sealed partial class NamePatternConditionalJobData : ObservableValidator, IJobEditorData
{
	private readonly JobCollection oldJobs;

	#region Pattern

	[NoErrors]
	public SearchTextData Pattern { get; } = new();

	private void OldString_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(HasErrors))
			ValidateProperty(Pattern, nameof(Pattern));
	}

	#endregion


	#region Constructors

	public NamePatternConditionalJobData()
	{
		Initialize();
	}

	public NamePatternConditionalJobData(ItemNameJobConditional job)
	{
		ArgumentNullException.ThrowIfNull(job, nameof(job));

		Pattern.TextType = /*job.UseRegex ? TextType.Regex :*/ TextType.Text;
		Pattern.IgnoreCase = job.IgnoreCase;
		Pattern.Text = job.Pattern;

		oldJobs = job.Jobs;

		Initialize();
	}

	private void Initialize()
	{
		Pattern.PropertyChanged += OldString_PropertyChanged;
		ValidateProperty(Pattern, nameof(Pattern));
	}

	#endregion


	public JobItem GetJobItem()
	{
		return oldJobs == null
			 ? new ItemNameJobConditional(Pattern.Text, Pattern.IgnoreCase/*, Pattern.TextType == TextType.Regex*/)
			 : new ItemNameJobConditional(Pattern.Text, Pattern.IgnoreCase/*, Pattern.TextType == TextType.Regex*/, oldJobs);
	}
}