using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CopyFileContents.Infrastructure.Util;

public class FileUtil {

	public static List<string> GetRelativePaths(List<string> paths) {
		if (paths == null || paths.Count == 0) {
			return [];
		}

		if (paths.Count == 1) {
			var singlePath = paths[0].Replace('/', '\\');
			return [Path.GetFileName(singlePath)];
		}

		var normalizedPaths = paths.Select(p => p.Replace('/', '\\')).ToList();

		var splitPaths = normalizedPaths.Select(p => p.Split(['\\'], StringSplitOptions.RemoveEmptyEntries)).ToList();

		var maxCommon = splitPaths.Min(p => p.Length);
		var common = 0;

		while (common < maxCommon && splitPaths.All(p =>
			string.Equals(p[common], splitPaths[0][common], StringComparison.OrdinalIgnoreCase))) {
			common++;
		}

		return splitPaths.Select(p => string.Join("\\", p.Skip(common))).ToList();
	}
}