namespace CopyFileContents.Models;

public record FileClipboard(string RelativePath, string FileContent, string Prefix) {
	
	public override string ToString() {
		var prefixFormatted = string.Empty;
		if (!string.IsNullOrWhiteSpace(Prefix)) {
			prefixFormatted = Prefix + " ";
		}
		return $"{prefixFormatted}{RelativePath}{Environment.NewLine}{Environment.NewLine}{FileContent}";
	}
}