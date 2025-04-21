using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Settings;
using Newtonsoft.Json;

namespace CopyFileContents.Infrastructure.Util;

public class FileUtil {

	public static List<string> GetRelativePaths(List<string> paths) {
		if (paths is null || !paths.Any()) {
			return [];
		}

		if (paths.Count == 1) {
			var singlePath = paths[0].Replace('/', '\\');
			return [Path.GetFileName(singlePath)];
		}

		var splitPaths = paths.Select(p => p.Replace('/', '\\'))
			.Select(p => p.Split(['\\'], StringSplitOptions.RemoveEmptyEntries)).ToImmutableList();
		var common = 0;

		while (common < splitPaths.Min(p => p.Length) && splitPaths.All(p =>
			string.Equals(p[common], splitPaths[0][common], StringComparison.OrdinalIgnoreCase))) {
			common++;
		}

		return splitPaths.Select(p => string.Join("\\", p.Skip(common))).ToList();
	}

	public static List<string> GetFullPaths(IEnumerable<SolutionItem> items) {
		if (items is null || !items.Any()) {
			return [];
		}

		var folders = GetFolders(items);
		var files = GetFiles(folders);

		AddFiles(items, SolutionItemType.PhysicalFile, files);
		AddFiles(items, SolutionItemType.Solution, files);
		AddFiles(items, SolutionItemType.Project, files);

		return files.Distinct().ToList();
	}

	private static void AddFiles(IEnumerable<SolutionItem> items, SolutionItemType fileType, List<string> files) => files.AddRange(items
			.Where(f => f.Type == fileType)
			.Select(f => f.FullPath)
			.Where(currentFile => !files.Any(file => file == currentFile)));

	private static List<string> GetFolders(IEnumerable<SolutionItem> items) {
		var folders = items
			.Where(f => f.Type == SolutionItemType.PhysicalFolder)
			.Select(f => Path.GetFullPath(f.FullPath).TrimEnd(Path.DirectorySeparatorChar))
			.Distinct()
			.ToList();

		return folders
			.Where(folder => !folders
			.Any(other => folder != other && folder.StartsWith(other + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)))
			.ToList();
	}

	private static List<string> GetFiles(List<string> folders) {
		List<string> files = [];
		foreach (var folder in folders) {
			Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
				.ToList()
				.ForEach(files.Add);
		}
		return files;
	}
}