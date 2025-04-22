using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CopyFileContents.Infrastructure.Util;

public class FileUtil {

	public static IEnumerable<string> GetRelativePathsFromCommonRoot(List<string> paths) {
		if (paths is null || paths.Count == 0) {
			return [];
		}

		if (paths.Count == 1) {
			var singlePath = paths[0].Replace('/', '\\');
			return [Path.GetFileName(singlePath)];
		}

		var splitPaths = paths.Select(p => p.Replace('/', '\\'))
			.Select(p => p.Split(['\\'], StringSplitOptions.RemoveEmptyEntries)).ToList();
		var common = 0;

		while (common < splitPaths.Min(p => p.Length) && splitPaths.All(p =>
			string.Equals(p[common], splitPaths[0][common], StringComparison.OrdinalIgnoreCase))) {
			common++;
		}

		return splitPaths.Select(p => string.Join("\\", p.Skip(common)));
	}

	internal static IEnumerable<string> GetAllFiles(IEnumerable<SolutionItem> items) {
		if (items is null || !items.Any()) {
			return [];
		}

		var folders = ExtractFoldersFromSolution(items);
		var files = ExtractFilesFromFolders(folders).ToList();

		AddFiles(items, SolutionItemType.PhysicalFile, files);
		AddFiles(items, SolutionItemType.Solution, files);
		AddFiles(items, SolutionItemType.Project, files);

		return files.Distinct();
	}

	private static void AddFiles(IEnumerable<SolutionItem> items, SolutionItemType fileType, List<string> files) => files.AddRange(items.Where(f => f.Type == fileType).Select(f => f.FullPath));

	private static IEnumerable<string> ExtractFoldersFromSolution(IEnumerable<SolutionItem> items) {
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

	private static IEnumerable<string> ExtractFilesFromFolders(IEnumerable<string> folders) {
		foreach (var folder in folders) {
			foreach (var file in EnumerateFilesRecursively(folder)) {
				yield return file;
			}
		}
	}

	private static IEnumerable<string> EnumerateFilesRecursively(string folder) {
		string[] files = [];
		try {
			files = Directory.GetFiles(folder);
		}
		catch (UnauthorizedAccessException ex) {
			ex.Log($"Access denied to folder {folder}: {ex.Message}");
			yield break;
		}
		catch (DirectoryNotFoundException ex) {
			ex.Log($"Folder not found {folder}: {ex.Message}");
			yield break;
		}
		catch (IOException ex) {
			ex.Log($"I/O error accessing files in folder {folder}: {ex.Message}");
			yield break;
		}

		foreach (var file in files) {
			yield return file;
		}

		string[] subfolders = [];
		try {
			subfolders = Directory.GetDirectories(folder);
		}
		catch (UnauthorizedAccessException ex) {
			ex.Log($"Access denied to subdirectories in folder {folder}: {ex.Message}");
			yield break;
		}
		catch (DirectoryNotFoundException ex) {
			ex.Log($"Subdirectory not found in folder {folder}: {ex.Message}");
			yield break;
		}
		catch (IOException ex) {
			ex.Log($"I/O error accessing subdirectories in folder {folder}: {ex.Message}");
			yield break;
		}

		foreach (var subfolder in subfolders) {
			foreach (var file in EnumerateFilesRecursively(subfolder)) {
				yield return file;
			}
		}
	}
}