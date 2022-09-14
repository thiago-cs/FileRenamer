using System.Text.RegularExpressions;
using System.Xml;
using FileRenamer.Core.Extensions;
using FileRenamer.Core.Serialization;


namespace FileRenamer.Core.Jobs.FileActions;

#if DEBUG
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
#endif
public sealed class ChangeStringCaseAction : RenameActionBase
{
	#region Properties and fields

	private Regex? regex;

	public string OldString { get; }
	public bool IgnoreCase { get; }
	public bool UseRegex { get; }
	public TextCasing TextCase { get; }

	#endregion


	public ChangeStringCaseAction(string oldString, bool ignoreCase, bool useRegex, TextCasing textCase)
	{
		OldString = oldString;
		IgnoreCase = ignoreCase;
		UseRegex = useRegex;
		TextCase = textCase;

		UpdateDescription();
	}


	#region RenameActionBase implementation

	public override void Run(JobTarget target, JobContext context)
	{
		string input = target.NewFileName;

		//1. Plain text
		if (!UseRegex)
		{
			target.NewFileName = input.Replace(
				OldString,
				OldString.ToCase(TextCase),
				IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);

			return;
		}

		// 2. REGEX
		// 2.1. Creates the Regex object, if necessary.
		if (regex == null)
		{
			RegexOptions regexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

			if (IgnoreCase)
				regexOptions |= RegexOptions.IgnoreCase;

			regex = new(OldString, regexOptions);
		}

		// 2.2. Applies the case conversion.
		IList<Match> matches = regex.Matches(input);

		foreach (Match match in matches)
			if (match.Value.Length != 0)
				input = input.Replace(match.Value, match.Value.ToCase(TextCase));

		// 2.3. 
		target.NewFileName = input;
	}

	public override void UpdateDescription()
	{
		string @case = TextCase switch
		{
			TextCasing.LowerCase => "lowercase",
			TextCasing.UpperCase => "uppercase",
			TextCasing.SentenceCase => "sentence case",
			TextCasing.TitleCase => "title case",
			TextCasing.TitleCaseIgnoreCommonWords => "title case (ignore common words)",
			_ => TextCase.ToString(),
		};

		System.Text.StringBuilder sb = new();

		sb.Append("change all occurrences of ");

		if (UseRegex)
			sb.Append("the expression ");

		sb.Append('"')
		  .Append(OldString)
		  .Append(@""" to ")
		  .Append(@case);

		if (IgnoreCase)
			sb.Append(@" (ignore case)");

		Description = sb.ToString();
	}

	public override RenameActionBase DeepCopy()
	{
		return new ChangeStringCaseAction(OldString, IgnoreCase, UseRegex, TextCase);
	}

	#endregion


	#region XML serialization

	public override async Task WriteXmlAsync(XmlWriter writer)
	{
		await writer.WriteStartElementAsync(GetType().Name).ConfigureAwait(false);

		await writer.WriteAttributeAsync(nameof(OldString), OldString).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(IgnoreCase), IgnoreCase).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(UseRegex), UseRegex).ConfigureAwait(false);
		await writer.WriteAttributeAsync(nameof(TextCase), TextCase).ConfigureAwait(false);

		await writer.WriteEndElementAsync().ConfigureAwait(false);
	}

	public static Task<RenameActionBase> ReadXmlAsync(XmlReader reader)
	{
		string? oldString = null;
		bool? ignoreCase = null;
		bool? useRegex = null;
		TextCasing? textCase = null;

		while (reader.MoveToNextAttribute())
			switch (reader.Name)
			{
				case nameof(OldString):
					oldString = reader.Value;
					break;

				case nameof(IgnoreCase):
					ignoreCase = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(UseRegex):
					useRegex = XmlSerializationHelper.ParseBoolean(reader.Value);
					break;

				case nameof(TextCase):
					textCase = Enum.Parse<TextCasing>(reader.Value);
					break;

				default:
					// Unknown attribute!?
					//Console.WriteLine($"Name: {reader.Name}, value: {reader.Value}");
					break;
			}

		reader.ReadStartElement(nameof(ChangeStringCaseAction));

		//
		XmlSerializationHelper.ThrowIfNull(oldString, nameof(OldString));
		XmlSerializationHelper.ThrowIfNull(ignoreCase, nameof(IgnoreCase));
		XmlSerializationHelper.ThrowIfNull(useRegex, nameof(UseRegex));
		XmlSerializationHelper.ThrowIfNull(textCase, nameof(TextCase));

		ChangeStringCaseAction result = new(oldString, ignoreCase.Value, useRegex.Value, textCase.Value);
		return Task.FromResult(result as RenameActionBase);
	}

	#endregion
}