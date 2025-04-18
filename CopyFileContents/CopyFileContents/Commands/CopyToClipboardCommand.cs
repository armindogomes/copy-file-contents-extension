using CopyFileContents.Infrastructure.Util;
using CopyFileContents.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CopyFileContents;

[Command(PackageIds.CopyToClipboardCommand)]
internal sealed class CopyToClipboardCommand : BaseCommand<CopyToClipboardCommand> {

	protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
		var items = await VS.Solutions.GetActiveItemsAsync();

		if (!items.Any()) {
			await VS.StatusBar.ShowMessageAsync("No file was selected. That's odd...");
			return;
		}

		await VS.StatusBar.ShowMessageAsync($"Starting to copy the content of {items.Count()} files to the clipboard");

		var existingItems = items.Where(i => File.Exists(i.FullPath)).ToList();
		var fullPaths = existingItems.Select(i => i.FullPath).ToList();
		var relativePaths = FileUtil.GetRelativePaths(fullPaths);

		var files = new List<FileClipboard>();
		for (int i = 0; i < fullPaths.Count; i++) {
			var content = File.ReadAllText(fullPaths[i]);
			files.Add(new FileClipboard(relativePaths[i], content));
		}

		if (files.Any()) {
			const string LINE = "----------------------------------------";
			try {
				Clipboard.SetText(string.Join(LINE + Environment.NewLine, files));
				await VS.StatusBar.ShowMessageAsync($"The content of {files.Count} files has been copied");
			}
			catch (Exception ex) {
				await ex.LogAsync();
			}
		}
	}
}