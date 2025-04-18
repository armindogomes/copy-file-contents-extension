namespace CopyFileContents.Models;

public record FileClipboard(string RelativePath, string FileContent) {

	public override string ToString() => $"# {RelativePath}{Environment.NewLine}{Environment.NewLine}{FileContent}";
}