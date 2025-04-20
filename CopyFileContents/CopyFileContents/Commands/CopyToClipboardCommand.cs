using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CopyFileContents.Infrastructure.Util;
using CopyFileContents.Models;

namespace CopyFileContents;

[Command(PackageIds.CopyToClipboardCommand)]
internal sealed class CopyToClipboardCommand : BaseCommand<CopyToClipboardCommand> {

	protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
		var items = await VS.Solutions.GetActiveItemsAsync();

		if (!items.Any()) {
			await VS.StatusBar.ShowMessageAsync("No file was selected. That's odd...");
			return;
		}

		await VS.StatusBar.ShowMessageAsync($"Starting to copy the content of {items.Count()} file(s) to the clipboard");

		var fullPaths = FileUtil.GetFullPaths(items);
		var relativePaths = FileUtil.GetRelativePaths(fullPaths);

		var files = new List<FileClipboard>();
		var sb = new StringBuilder();
		for (var i = 0; i < fullPaths.Count; i++) {
			try {
				sb.Append(File.ReadAllText(fullPaths[i]));
				if (!sb.ToString().All(c => !char.IsControl(c) || c == '\n' || c == '\r' || c == '\t' || c == ' ')) {
					throw new Exception("This is not a text file");
				}
				files.Add(new FileClipboard(relativePaths[i], sb.ToString()));
			}
			catch (ArgumentOutOfRangeException ex) {
				ex.Log($"Error processing file {fullPaths[i]}: Possible capacity overflow in StringBuilder - {ex.Message}");
			}
			catch (Exception ex) {
				ex.Log($"Error processing file {fullPaths[i]}: {ex.Message}");
			}
			finally {
				sb.Clear();
			}
		}

		await ClipboardUtil.WriteToClipboardAsync(files);
	}
}