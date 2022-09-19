using System.IO;
using NUnit.Framework;
using FileRenamer.Core;
using FileRenamer.Core.Extensions;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.ValueSources;
using FileRenamer.Core.ValueSources.NumberFormatters;


namespace FileRenamer.Core_Tester;

[TestFixture]
public static class Test_Project
{
	[Test]
	public static async void TestRun1()
	{
		JobCollection jobCollection = new()
		{
			new InsertAction(new BeginningIndex(), new StringValueSource("My name is ")),
			new InsertAction(new BeginningIndex(), new RandomStringValueSource() { IncludeLowercase = true, IncludeUppercase = false, IncludeNumbers = true, IncludeSymbols = false, Length = 28 }),
			new InsertAction(new BeginningIndex(), new FolderNameValueSource()),
			new InsertAction(new EndIndex(), new CounterValueSource() { InitialValue = 0, Increment = 1 }),
			new InsertAction(new EndIndex(), new CounterValueSource() { InitialValue = 1, Increment = 2, Formatter = new PaddedNumberFormatter() { MinWidth = 5, PaddingChar = 't' } }),
			new InsertAction(new EndIndex(), new CounterValueSource() { InitialValue = 2, Increment = 3, Formatter = new NumberToWordsFormatter() { Gender = Humanizer.GrammaticalGender.Feminine, UseUppercase = false } }),
			new InsertAction(new EndIndex(), new CounterValueSource() { InitialValue = 3, Increment = 4, Formatter = new RomanNumberFormatter() { UseUppercase = true } }),
			new RemoveAction(new SubstringIndex("Jack Pearson", true, false, true), 8),
			new ChangeRangeCaseAction(new FixedIndex(1), new FileExtensionIndex(), TextCasing.TitleCase),
		};

		Project project = new(jobCollection);

		using MemoryStream writeStream = new();
		await project.WriteXmlAsync(writeStream);

		writeStream.Position = 0;
		using StreamReader readStream = new(writeStream, System.Text.Encoding.UTF8);
		Project project2 = await Project.ReadXmlAsync(readStream);


		Assert.AreEqual(project.Jobs.Count, project2.Jobs.Count);
	}
}