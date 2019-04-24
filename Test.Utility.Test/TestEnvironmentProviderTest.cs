using System.IO;
using Xunit;

namespace Messerli.Test.Utility.Test
{
    public class TestEnvironmentProviderTest
    {
        private const string ResourcesFolder = "Resources";

        [Fact]
        public void CreateParentDirectoryTest()
        {
            using (var generateFileStructure = new TestEnvironmentProvider())
            {
                var tempPath = Path.GetTempPath();
                var randomDirectoryName = generateFileStructure.RootDirectory;
                var path = Path.Combine(tempPath, randomDirectoryName);
                Assert.True(Directory.Exists(path));
            }
        }

        [Fact]
        public void CreatedChildFileExists()
        {
            var testFiles = new[]
            {
                new TestFile("file1.txt")
            };

            using (var generateFileStructure = new TestEnvironmentProvider(testFiles))
            {
                var tempPath = Path.GetTempPath();
                var path = Path.Combine(tempPath, generateFileStructure.RootDirectory, testFiles[0].RelativeFilePath);

                Assert.True(File.Exists(path));
            }
        }

        [Fact]
        public void CreatedChildrenExist()
        {
            var testFiles = new[]
            {
                new TestFile("file1.txt"),
                new TestFile("file2.txt"),
                new TestFile("file3.txt")
            };

            using (var generateFileStructure = new TestEnvironmentProvider(testFiles))
            {
                var tempPath = Path.GetTempPath();

                foreach (var file in testFiles)
                {
                    Assert.True(File.Exists(Path.Combine(tempPath, generateFileStructure.RootDirectory, file.RelativeFilePath)));
                }

            }
        }

        [Fact]
        public void IsParentDirectoryDeleted()
        {
            var testFiles = new[]
            {
                new TestFile("file1.txt"),
                new TestFile("file2.txt"),
                new TestFile("file3.txt")
            };

            string path;
            using (var generateFileStructure = new TestEnvironmentProvider(testFiles))
            {
                path = generateFileStructure.RootDirectory;

                Assert.True(Directory.Exists(path));
            }

            Assert.False(Directory.Exists(path));
        }

        [Fact]
        public void HasCopiedFilesWithSameContent()
        {
            var testFiles = new[]
            {
                new TestFile("file1.txt"),
                new TestFile("file2.txt"),
                new TestFile("file3.txt")
            };

            using (var generateFileStructure = new TestEnvironmentProvider(testFiles))
            {
                foreach (var file in testFiles)
                {
                    IsSame(Path.Combine(ResourcesFolder, file.SourceFilePath), Path.Combine(generateFileStructure.RootDirectory, file.RelativeFilePath));
                }
            }
        }

        [Fact]
        public void HasCopiedFilesWithSameContentInSubfolders()
        {
            var testFiles = new[]
            {
                new TestFile("file1.txt", "Foo1/file1.txt"),
                new TestFile("file2.txt", "Foo2/file2.txt"),
                new TestFile("file3.txt", "Foo3/SubFoo3/file3.txt")
            };

            using (var generateFileStructure = new TestEnvironmentProvider(testFiles))
            {
                foreach (var files in testFiles)
                {
                    IsSame(Path.Combine(ResourcesFolder, files.SourceFilePath), Path.Combine(generateFileStructure.RootDirectory, files.RelativeFilePath));
                }
            }
        }

        [Fact]
        public void ThrowsAnExceptionOnInvalidPath()
        {
            var testFiles = new[]
            {
                new TestFile("file5.txt", "file1.txt")
            };

            Assert.Throws<FileNotFoundException>(() => new TestEnvironmentProvider(testFiles));
        }

        [Fact]
        public void HasCopiedFilesInSubfolder()
        {
            var testFiles = new[]
            {
                new TestFile("SubFolder4/file4.txt", "file1.txt")
            };

            using (var generateFileStructure = new TestEnvironmentProvider(testFiles))
            {
                foreach (var files in testFiles)
                {
                    IsSame(Path.Combine(ResourcesFolder, files.SourceFilePath), Path.Combine(generateFileStructure.RootDirectory, files.RelativeFilePath));
                }

                Assert.True(File.Exists(Path.Combine(generateFileStructure.RootDirectory, testFiles[0].RelativeFilePath)));
            }
        }

        [Fact]
        public void RemovesReadonlyFile()
        {
            var testFiles = new[]
            {
                new TestFile("file1.txt")
            };

            using (var generateFileStructure = new TestEnvironmentProvider(testFiles))
            {
                var tempPath = Path.GetTempPath();
                var path = Path.Combine(tempPath, generateFileStructure.RootDirectory, testFiles[0].RelativeFilePath);

                File.SetAttributes(path, FileAttributes.ReadOnly);
            }
        }

        private static void IsSame(string sourcePath, string destinationPath)
        {
            var testFile = File.ReadAllText(sourcePath);
            var createdFile = File.ReadAllText(destinationPath);

            Assert.Equal(testFile, createdFile);
        }

    }
}