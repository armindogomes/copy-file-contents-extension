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
}