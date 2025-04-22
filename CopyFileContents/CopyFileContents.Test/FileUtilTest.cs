using CopyFileContents.Infrastructure.Util;

namespace CopyFileContents.Test;

public class FileUtilTests {

	[Fact]
	public void Test01() {
		var input = new List<string> {
			@"C:\Users\user\Documents\file1.txt",
			@"C:\Users\user\Documents\file2.txt",
			@"C:\Users\user\Documents\file3.txt"
		};

		var expected = new List<string> {
			@"file1.txt",
			@"file2.txt",
			@"file3.txt"
		};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}

	[Fact]
	public void Test02() {
		var input = new List<string> {
			@"C:\Project\src\main.cs",
			@"C:\Project\src\test\test1.cs"
		};

		var expected = new List<string> {
			@"main.cs",
			@"test\test1.cs"
		};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}

	[Fact]
	public void Test03() {
		var input = new List<string> {
			@"C:\a\b\c\file.txt",
			@"C:\a\b\c\d\e\file2.txt"
		};

		var expected = new List<string> {
			@"file.txt",
			@"d\e\file2.txt"
		};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}

	[Fact]
	public void Test04() {
		var input = new List<string> {
			@"D:\data\file.csv",
			@"D:\data\archive\2023\file2.csv",
			@"D:\data\archive\2024\file3.csv"
		};

		var expected = new List<string> {
			@"file.csv",
			@"archive\2023\file2.csv",
			@"archive\2024\file3.csv"
		};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}

	[Fact]
	public void Test05() {
		var input = new List<string> {
			@"C:\A\B\C\test1.txt",
			@"C:\A\B\D\test2.txt"
		};

		var expected = new List<string> {
			@"C\test1.txt",
			@"D\test2.txt"
		};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}

	[Fact]
	public void Test06() {
		var input = new List<string> {
			@"C:\shared\images\img1.png",
			@"C:\shared\videos\vid1.mp4",
			@"C:\shared\docs\doc1.pdf"
		};

		var expected = new List<string> {
			@"images\img1.png",
			@"videos\vid1.mp4",
			@"docs\doc1.pdf"
		};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}

	[Fact]
	public void Test07() {
		var input = new List<string> {
			@"C:\data\file.txt",
			@"D:\data\file.txt"
		};

		var expected = new List<string> {
			@"C:\data\file.txt",
			@"D:\data\file.txt"
		};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}

	[Fact]
	public void Test08() {
		var input = new List<string> {
			@"C:\dir\sub\one.txt",
			@"C:\dir\sub\sub2\two.txt",
			@"C:\dir\sub\sub2\three\three.txt"
		};

		var expected = new List<string> {
			@"one.txt",
			@"sub2\two.txt",
			@"sub2\three\three.txt"
		};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}

	[Fact]
	public void Test09() {
		var input = new List<string> {
			@"C:\A\B\C\file.txt",
			@"C:/A/B/C/file2.txt" // barra mista
        };

		var expected = new List<string> {
			@"file.txt",
			@"file2.txt"
		};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}

	[Fact]
	public void Test10() {
		var input = new List<string> {
				@"C:\onlyone\file.txt"
			};

		var expected = new List<string> {
				@"file.txt"
			};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}

	[Fact]
	public void Test11() {
		var input = new List<string> {
				@"C:\onlyone\file1",
				@"C:\onlyone\file2"
			};

		var expected = new List<string> {
				@"file1",
				@"file2"
			};

		var output = FileUtil.CreateRelativePaths(input);
		Assert.Equal(expected, output);
	}
}