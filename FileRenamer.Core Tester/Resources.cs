﻿using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.ValueSources;
using FileRenamer.Core.ValueSources.NumberFormatters;


namespace FileRenamer.Core_Tester;

internal static class Resources
{
	public const string quickBrownFox = "the quick brown fox jumps over the lazy dog.";
	public const string fiveBoxingWizards = "The five boxing wizards jump quickly.";
	public const string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam at sem consectetur, egestas velit vitae, lacinia ipsum.";
	public const string neverForget = "Forget injuries, never forget kindnesses";
	public const string quickBrownNinjaCat = "the quick brown ninja cat jumps over the lazy dog.";

	public static readonly string[] testStrings = new[]
	{
		string.Empty,
		loremIpsum,
		quickBrownFox,
		fiveBoxingWizards,
		neverForget,
	};

	public static readonly string[] texts =
	{
		"a tale of two cities",
		"gROWL to the rescue",
		"sports and MLB baseball",
		"UNICEF and children",
		"UNICEF AND CHILDREN",
	};

	public static readonly string[] fileNames =
	{
		"new document.txt",
		"Area.effect",
	};

	// language=regex
	public const string regex1 = @",[^\.]*(?=,)";

	public static readonly IIndex[] finders =
	{
		// 0
		new BeginningIndex(),
		new FixedIndex(3),
		new SubstringIndex("sunset", true, false, false),
		new SubstringIndex("dark", false, false, false),
		new SubstringIndex("(Hi|Hello) kitty", false, false, true),
		// 5
		new FileExtensionIndex(),
		new EndIndex(),
		new FixedIndex(-2),
		new FixedIndex(-1),
		new FixedIndex(0),
		// 10
		new FixedIndex(1),
	};

	public static readonly RenameFileJob[] actions =
	{
		// 0
		new InsertAction(new BeginningIndex(), (StringValueSource)"Once upon a time,"),
		new InsertAction(new EndIndex(), (StringValueSource)"The End."),
		new RemoveAction(new BeginningIndex(), new EndIndex()),
		new RemoveAction(new FixedIndex(7), 2),
		new InsertAction(new SubstringIndex("episode", false, false, false), new CounterValueSource() { InitialValue = 1, Formatter = new PaddedNumberFormatter() { MinWidth = 2 } }),
		// 5
		new ReplaceAction("out with the old", "in with the new", false, true),
		new ReplaceAction(new FixedIndex(3), new FileExtensionIndex(), "out with the old", "in with the new", false, true),
		new ChangeRangeCaseAction(new BeginningIndex(), new FileExtensionIndex(), Core.Extensions.TextCasing.TitleCaseIgnoreCommonWords),
	};

	public static JobContext NoContext { get; } = new(new(), System.Array.Empty<JobTarget>());
}