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


	public static readonly IIndexFinder[] finders =
	{
		new BeginningIndexFinder(),
		new FixedIndexFinder(3),
		new SubstringIndexFinder("sunset", true, false, false),
		new SubstringIndexFinder("dark", false, false, false),
		new SubstringIndexFinder("(Hi|Hello) kitty", false, false, true),
		new FileExtensionIndexFinder(),
		new EndIndexFinder(),
	};
}