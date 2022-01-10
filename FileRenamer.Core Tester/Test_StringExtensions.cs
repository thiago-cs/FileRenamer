using NUnit.Framework;
using FileRenamer.Core.Extensions;
using static FileRenamer.Core_Tester.Resources;


namespace FileRenamer.Core_Tester;

[TestFixture]
public sealed class Test_StringExtensions
{
	[Test]
	[TestCase(1, TextCasing.TitleCase, "A Tale Of Two Cities")]
	[TestCase(2, TextCasing.TitleCase, "GROWL To The Rescue")]
	[TestCase(3, TextCasing.LowerCase, "sports and mlb baseball")]
	[TestCase(3, TextCasing.UpperCase, "SPORTS AND MLB BASEBALL")]
	[TestCase(3, TextCasing.SentenceCase, "Sports and MLB baseball")]
	[TestCase(3, TextCasing.TitleCase, "Sports And MLB Baseball")]
	[TestCase(4, TextCasing.LowerCase, "unicef and children")]
	[TestCase(4, TextCasing.UpperCase, "UNICEF AND CHILDREN")]
	[TestCase(4, TextCasing.SentenceCase, "UNICEF and children")]
	[TestCase(4, TextCasing.TitleCase, "UNICEF And Children")]
	[TestCase(5, TextCasing.SentenceCase, "Unicef and children")]
	[TestCase(5, TextCasing.TitleCase, "Unicef And Children")]
	public void Test(int textIndex, TextCasing textCase, string expected)
	{
		Assert.AreEqual(expected, texts[textIndex - 1].ToCase(textCase));
		Assert.Pass();
	}
}