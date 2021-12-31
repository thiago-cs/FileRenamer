using NUnit.Framework;
using FileRenamer.Core.Extensions;


namespace FileRenamer.Core_Tester;

public sealed class Test_StringExtensions
{
	private const string text1 = "a tale of two cities";
	private const string text2 = "gROWL to the rescue";
	private const string text3 = "sports and MLB baseball";
	private const string text4 = "UNICEF and children";
	private const string text5 = "UNICEF AND CHILDREN";


	[Test]
	[TestCase(text1, TextCasing.TitleCase, "A Tale Of Two Cities")]
	[TestCase(text2, TextCasing.TitleCase, "GROWL To The Rescue")]
	[TestCase(text3, TextCasing.LowerCase, "sports and mlb baseball")]
	[TestCase(text3, TextCasing.UpperCase, "SPORTS AND MLB BASEBALL")]
	[TestCase(text3, TextCasing.SentenceCase, "Sports and MLB baseball")]
	[TestCase(text3, TextCasing.TitleCase, "Sports And MLB Baseball")]
	[TestCase(text4, TextCasing.LowerCase, "unicef and children")]
	[TestCase(text4, TextCasing.UpperCase, "UNICEF AND CHILDREN")]
	[TestCase(text4, TextCasing.SentenceCase, "UNICEF and children")]
	[TestCase(text4, TextCasing.TitleCase, "UNICEF And Children")]
	[TestCase(text5, TextCasing.SentenceCase, "Unicef and children")]
	[TestCase(text5, TextCasing.TitleCase, "Unicef And Children")]
	public void Test(string input, TextCasing textCase, string expected)
	{
		Assert.AreEqual(expected, input.ToCase(textCase));
		Assert.Pass();
	}
}