using System.ComponentModel;


namespace FileRenamer.UserControls.InputControls;

public enum ValueSourceType
{
	[Description("a text")]
	FixedText,

	[Description("a random text")]
	RandomText,

	[Description("a counter")]
	Counter,

	//[Description("folder info")]
	//FolderInfo,

	//[Description("MP3 tag")]
	//Mp3Tag,

	//[Description("EXIF tag")]
	//ExifTag,
}