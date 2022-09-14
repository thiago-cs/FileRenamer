using System.Diagnostics.CodeAnalysis;
using System.Xml;
using FileRenamer.Core.Indices;
using FileRenamer.Core.Jobs;
using FileRenamer.Core.Jobs.FileActions;
using FileRenamer.Core.ValueSources;
using FileRenamer.Core.ValueSources.NumberFormatters;


namespace FileRenamer.Core.Serialization;

public sealed class XmlSerializationHelper
{
	internal static bool ParseBoolean(string value)
	{
		return value.ToLowerInvariant() == "true";
	}

	internal static void ThrowIfNull<T>([NotNull] T? value, string property, [System.Runtime.CompilerServices.CallerFilePath] string? path = null)
	{
		if (value == null)
			throw new XmlException($"A value for the {Path.GetFileNameWithoutExtension(path)}.{property} property must be specified in XML.");
	}
}