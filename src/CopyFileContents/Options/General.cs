using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CopyFileContents;

internal partial class OptionsProvider {

	[ComVisible(true)]
	public class GeneralOptions : BaseOptionPage<General> { }
}

public class General : BaseOptionModel<General> {

	private const string CATEGORY = "General";
	private const int FILENAME_PREFIX_LIMIT = 15;

	[Category(CATEGORY)]
	[DisplayName("Separator")]
	[Description("Specifies the separator to use between the contents of each file.")]
	[DefaultValue(SeparatorType.Dashes)]
	[TypeConverter(typeof(EnumConverter))]
	public SeparatorType Separator { get; set; } = SeparatorType.Dashes;

	public string _filePrefix = "#";

	[Category(CATEGORY)]
	[DisplayName("Filename prefix")]
	[Description("Specifies the prefix to add before each filename. Maximum length: 15 characters.")]
	[DefaultValue("#")]
	public string FilePrefix {
		get => _filePrefix;
		set {
			_filePrefix = (value.Length > FILENAME_PREFIX_LIMIT ? value.Substring(0, FILENAME_PREFIX_LIMIT) : value).Trim();
		}
	}
}

public enum SeparatorType {
	None,
	Dashes,
	Underscores
}