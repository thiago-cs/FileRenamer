using FileRenamer.Core.Actions;
using FileRenamer.Core.Indices;


namespace FileRenamer.Core_Tester;

public static class Resources
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
		new BeginningIndex(),
		new FixedIndex(3),
		new SubstringIndex("sunset", true, false, false),
		new SubstringIndex("dark", false, false, false),
		new SubstringIndex("(Hi|Hello) kitty", false, false, true),
		new FileExtensionIndex(),
		new EndIndex(),
	};

	internal static readonly RenameActionBase[] actions =
	{
		new InsertAction(new BeginningIndex(), "Once upon a time,"),
		new InsertAction(new EndIndex(), "The End."),
		new RemoveAction(new BeginningIndex(), new EndIndex()),
		new RemoveAction(new FixedIndex(7), 2),
		new InsertCounterAction(new SubstringIndex("episode", false, false, false),  1, 2),
		new ReplaceAction("out with the old", "in with the new", false, true),
		new ReplaceAction(new FixedIndex(3), new FileExtensionIndex(), "out with the old", "in with the new", false, true),
		new ToCaseAction(new BeginningIndex(), new FileExtensionIndex(), Core.Extensions.TextCasing.TitleCaseIgnoreCommonWords),
	};
}