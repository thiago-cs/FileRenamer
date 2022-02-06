namespace FileRenamer.Core.Indices;

public sealed class FileExtensionIndex : IIndex
{
	public IndexDescription Description { get; private set; }


	public FileExtensionIndex()
	{
		Description = new("before", "file's extension");
	}


	public int FindIn(string input)
	{
		for (int i = input.Length - 1; i >= 0; i--)
			switch (input[i])
			{
				case '.': return i;
				case ' ': return -1;
			}

		return -1;
	}
}