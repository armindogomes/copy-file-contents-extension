using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CopyFileContents.Models;

namespace CopyFileContents.Infrastructure.Util;

public class ClipboardUtil {

	public static async Task WriteToClipboardAsync(IEnumerable<FileClipboard> files) {
		if (files.Any()) {
			const string LINE = "----------------------------------------";
			string separator = Environment.NewLine + LINE + Environment.NewLine;
			try {
				Clipboard.SetText(string.Join(separator, files));
				await VS.StatusBar.ShowMessageAsync($"The content of {files.Count()} file(s) has been copied to the clipboard");
			}
			catch (Exception ex) {
				ex.Log($"An unexpected error occurred while copying content to the clipboard: {ex.Message}");
			}
		}
	}
}