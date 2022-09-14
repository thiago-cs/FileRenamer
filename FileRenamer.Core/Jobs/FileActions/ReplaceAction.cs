using System.Text.RegularExpressions;
using System.Xml;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class ReplaceAction : RenameActionBase
{
	#region Properties and fields

	private Regex? regex;

	public IIndex? StartIndex { get; }
	public IIndex? EndIndex { get; }
	public string OldString { get; }
	public string? NewString { get; }
	public bool IgnoreCase { get; }
	public bool UseRegex { get; }

	#endregion


	public ReplaceAction(string oldString, string? newString, bool ignoreCase, bool useRegex)
	{
		OldString = oldString ?? throw new ArgumentNullException(nameof(oldString));
		NewString = newString;
		IgnoreCase = ignoreCase;
		UseRegex = useRegex;

		UpdateDescription();
	}

	public ReplaceAction(IIndex startIndex, IIndex endIndex, string oldString, string? newString, bool ignoreCase, bool useRegex)
	{
		// 1. 
		StartIndex = startIndex ?? throw new ArgumentNullException(nameof(startIndex));
		EndIndex = endIndex ?? throw new ArgumentNullException(nameof(endIndex));
		OldString = oldString ?? throw new ArgumentNullException(nameof(oldString));
		NewString = newString;
		IgnoreCase = ignoreCase;
		UseRegex = useRegex;

		UpdateDescription();
	}


	#region RenameActionBase implementation

	public override void Run(JobTarget target, JobContext context)
	{
		string input = target.NewFileName;

		// 
		if (StartIndex == null || EndIndex == null)
		{
			target.NewFileName = Run_Core(input);
			return;
		}

		//
		int startIndex = StartIndex.FindIn(input);

		if (startIndex == -1)
			return;

		int endIndex = EndIndex.FindIn(input);

		if (endIndex == -1)
			return;

		if (endIndex < startIndex)
			return;

		// 
		target.NewFileName = input[..startIndex]
						   + Run_Core(input[startIndex..endIndex])
						   + input[endIndex..];
	}

	public override void UpdateDescription()
	{
		bool newIsEmpty = string.IsNullOrEmpty(NewString);
		System.Text.StringBuilder sb = new();

		sb.Append(newIsEmpty ? "remove all occurrencies of " : "replace ");

		if (UseRegex)
			sb.Append("the expression ");

		sb.Append('"').Append(OldString).Append('"');

		if (!newIsEmpty)
			sb.Append(@" with """).Append(NewString).Append('"');

		if (StartIndex != null && EndIndex != null)
			sb.Append(" within ").Append(Indices.Range.GetDescription(StartIndex, EndIndex));

		if (IgnoreCase)
			sb.Append(" (ignore case)");

		Description = sb.ToString();
	}

	public override RenameActionBase DeepCopy()
	{
		return StartIndex != null && EndIndex != null
				? new ReplaceAction(StartIndex, EndIndex, OldString, NewString, IgnoreCase, UseRegex)
				: new ReplaceAction(OldString, NewString, IgnoreCase, UseRegex);
	}

	private string Run_Core(string input)
	{
		if (UseRegex)
		{
			if (regex == null)
			{
				RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

				if (IgnoreCase)
					regexOptions |= RegexOptions.IgnoreCase;

				regex = new(OldString, regexOptions);
			}

			return regex.Replace(input, NewString ?? "");
		}
		else
			return input.Replace(OldString, NewString, IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
	}

	#endregion


	#region XML serialization

	public override async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(IsEnabled), IsEnabled).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(OldString), OldString).ConfigureAwait(false);

		if (!string.IsNullOrEmpty(NewString))
			await writer.WriteAttributeAsync(nameof(NewString), NewString).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(IgnoreCase), IgnoreCase).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(UseRegex), UseRegex).ConfigureAwait(false);


		if (StartIndex != null && EndIndex != null)
		{
			await writer.WriteElementAsync(nameof(StartIndex), StartIndex).ConfigureAwait(false);
			await writer.WriteElementAsync(nameof(EndIndex), EndIndex).ConfigureAwait(false);
		}

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static async Task<RenameActionBase> ReadXmlAsync(XmlReader reader)
	{
		bool isEnable = true;
		string? oldString = null;
		string? newString = null;
		bool? ignoreCase = null;
		bool? useRegex = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(IsEnabled):
					isEnable = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(OldString):
					oldString = reader.Value;
					break;

				case nameof(NewString):
					newString = reader.Value;
					break;

				case nameof(IgnoreCase):
					ignoreCase = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(UseRegex):
					useRegex = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		var depth = reader.Depth;
		reader.ReadStartElement(nameof(ReplaceAction));

		//
		IIndex? startIndex = null;
		IIndex? endIndex = null;

		if (reader.Depth == depth)
		{
			while (reader.NodeType != XmlNodeType.EndElement)
				switch (reader.Name)
				{
					case nameof(StartIndex):
						reader.ReadStartElement();
						startIndex = await reader.ReadIIndexAsync().ConfigureAwait(false);
						reader.ReadEndElement();
						break;

					case nameof(EndIndex):
						reader.ReadStartElement();
						endIndex = await reader.ReadIIndexAsync().ConfigureAwait(false);
						reader.ReadEndElement();
						break;

					default:
						break;
				}

			reader.ReadEndElement();
		}

		//
		XmlSerializationHelper.ThrowIfNull(oldString, nameof(OldString));
		XmlSerializationHelper.ThrowIfNull(ignoreCase, nameof(IgnoreCase));
		XmlSerializationHelper.ThrowIfNull(useRegex, nameof(UseRegex));

		return startIndex == null || endIndex == null
			? new ReplaceAction(oldString, newString, ignoreCase.Value, useRegex.Value) { IsEnabled = isEnable }
			: new ReplaceAction(startIndex, endIndex, oldString, newString, ignoreCase.Value, useRegex.Value) { IsEnabled = isEnable };
	}

	#endregion
}