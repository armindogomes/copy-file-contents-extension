using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace CopyFileContents.Infrastructure.Util;

public class FileUtil {

	public static List<string> GetRelativePaths(List<string> paths) {
		if (paths == null || !paths.Any()) {
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

		var folders = items
			.Where(f => f.Type == SolutionItemType.PhysicalFolder)
			.Select(f => Path.GetFullPath(f.FullPath).TrimEnd(Path.DirectorySeparatorChar))
			.Distinct()
			.ToList();

		folders = folders
			.Where(folder => !folders.Any(other =>
				folder != other &&
				folder.StartsWith(other + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)))
			.ToList();

		List<string> files = [];
		foreach (var folder in folders) {
			Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
				.ToList()
				.ForEach(files.Add);
		}

		files.AddRange(items
			.Where(f => f.Type == SolutionItemType.PhysicalFile)
			.Select(f => f.FullPath)
			.Where(currentFile => !files.Any(file => file == currentFile))
			.ToList());

		return files.Distinct().ToList();
	}
}