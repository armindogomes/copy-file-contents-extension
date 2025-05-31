namespace CopyFileContents.Models;

public record FileClipboard(string RelativePath, string FileContent, string prefix) {

	public override string ToString() => $"{prefix} {RelativePath}{Environment.NewLine}{Environment.NewLine}{FileContent}";
}