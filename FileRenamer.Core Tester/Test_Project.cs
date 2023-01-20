using System.IO;
using System.Threading.Tasks;
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
	public static async Task TestRun1()
	{
		// 1. Tests serializing/deserializing a project.
		// 1.1. Creates a test subject.
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
		byte[] bytes;

		// 1.2. Serializes and then deserializes the project.
		{
			using MemoryStream memoryStream = new();
			await project.WriteXmlAsync(memoryStream);

			bytes = memoryStream.ToArray();

			memoryStream.Position = 0;
			using StreamReader readStream = new(memoryStream, System.Text.Encoding.UTF8);
			Project copyProject = await Project.ReadXmlAsync(readStream);
			memoryStream.SetLength(bytes.Length);
			AssertAreEqual(project, copyProject);
		}

		// 2. Replaces the first action with one that yields a smaller XML when serialized.
		// 2.1.
		jobCollection[0] = new InsertAction(new BeginningIndex(), new StringValueSource("I am "));

		// 2.2. Serializes and then deserializes the project.
		try
		{
			using MemoryStream memoryStream = new(bytes);
			await project.WriteXmlAsync(memoryStream);

			bytes = memoryStream.ToArray();

			memoryStream.Position = 0;
			using StreamReader readStream = new(memoryStream, System.Text.Encoding.UTF8);
			Project copyProject = await Project.ReadXmlAsync(readStream);

			AssertAreEqual(project, copyProject);
		}
		catch (System.Xml.XmlException)
		{
			System.Console.WriteLine(System.Text.Encoding.UTF8.GetChars(bytes));
			throw;
		}
	}

	private static void AssertAreEqual(Project p1, Project p2)
	{
		Assert.AreEqual(p1.Jobs.Count, p2.Jobs.Count);
	}
}