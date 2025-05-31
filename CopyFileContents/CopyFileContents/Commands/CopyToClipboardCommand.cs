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
		var relativePaths = FileUtil.GetRelativePathsFromCommonRoot(files).ToList();

		var contentToClipboard = new List<FileClipboard>();
		var sb = new StringBuilder();		
		var general = await General.GetLiveInstanceAsync();

		for (var i = 0; i < files.Count; i++) {
			try {
				sb.Append(File.ReadAllText(files[i]));
				if (!sb.ToString().All(c => !char.IsControl(c) || c == '\n' || c == '\r' || c == '\t' || c == ' ')) {
					throw new Exception("This is not a text file");
				}
				contentToClipboard.Add(new FileClipboard(relativePaths[i], sb.ToString().Trim(), general.FilePrefix));
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

		await ClipboardUtil.WriteToClipboardAsync(contentToClipboard, GetSeparator(general.Separator));
	}

	private static string GetSeparator(SeparatorType separatorType) {
		const string SEPARATOR_DASH = "----------------------------------------";
		const string SEPARATOR_UNDERSCORE = "________________________________________";
		return separatorType switch {
			SeparatorType.Dashes => Environment.NewLine + Environment.NewLine + SEPARATOR_DASH + Environment.NewLine + Environment.NewLine,
			SeparatorType.Underscores => Environment.NewLine + SEPARATOR_UNDERSCORE + Environment.NewLine + Environment.NewLine,
			_ => Environment.NewLine + Environment.NewLine,
		};
	}
}