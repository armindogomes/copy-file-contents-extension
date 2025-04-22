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

		if (items is null || !items.Any()) {
			await VS.StatusBar.ShowMessageAsync("No file was selected. That's odd...");
			return;
		}

		await VS.StatusBar.ShowMessageAsync($"Starting to copy the content of {items.Count()} file(s) to the clipboard");

		var files = FileUtil.GetAllFiles(items).ToList();
		var relativePaths = FileUtil.CreateRelativePaths(files).ToList();

		var contentToClipboard = new List<FileClipboard>();
		var sb = new StringBuilder();
		for (var i = 0; i < files.Count; i++) {
			try {
				sb.Append(File.ReadAllText(files[i]));
				if (!sb.ToString().All(c => !char.IsControl(c) || c == '\n' || c == '\r' || c == '\t' || c == ' ')) {
					throw new Exception("This is not a text file");
				}
				contentToClipboard.Add(new FileClipboard(relativePaths[i], sb.ToString()));
			}
			catch (ArgumentOutOfRangeException ex) {
				ex.Log($"Error processing file {files[i]}: Possible capacity overflow in StringBuilder - {ex.Message}");
			}
			catch (Exception ex) {
				ex.Log($"Error processing file {files[i]}: {ex.Message}");
			}
			finally {
				sb.Clear();
			}
		}

		await ClipboardUtil.WriteToClipboardAsync(contentToClipboard);
	}
}