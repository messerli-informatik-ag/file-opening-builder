using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Messerli.Test.Utility
{
    public class TestEnvironmentProvider : IDisposable
    {
        private const string ResourceDirectoryName = "Resources";

        private readonly IReadOnlyCollection<TestFile> _testFiles;

        public string RootDirectory { get; }

        public TestEnvironmentProvider(IReadOnlyCollection<TestFile> testFiles)
        {
            _testFiles = testFiles;

            try
            {
                RootDirectory = GenerateParent();

                GenerateChildren();
            }
            catch (Exception)
            {
                Dispose();
                throw;
            }
        }

        public TestEnvironmentProvider()
        {
            _testFiles = new TestFile[0];

            try
            {
                RootDirectory = GenerateParent();
            }
            catch (Exception)
            {
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            foreach (var testFile in _testFiles)
            {
                var destinationPath = GetDestinationPath(testFile);
                File.SetAttributes(destinationPath, FileAttributes.Normal);
            }

            Directory.Delete(RootDirectory, true);
        }

        private static string GenerateParent()
        {
            var tempPath = Path.GetTempPath();
            var randomDirectory = Guid.NewGuid().ToString();
            var path = Path.Combine(tempPath, randomDirectory);

            Directory.CreateDirectory(path);

            return path;
        }

        private void GenerateChildren()
        {
            if (HasSubDirectories())
            {
                CreateSubDirectories();
            }

            foreach (var file in _testFiles)
            {
                var sourcePath = GetSourcePath(file);
                var destinationPath = GetDestinationPath(file);

                File.Copy(sourcePath, destinationPath, true);
            }
        }

        private string GetSourcePath(TestFile testFile)
        {
            return Path.Combine(ResourceDirectoryName, testFile.SourceFilePath);
        }

        private string GetDestinationPath(TestFile testFile)
        {
            return Path.Combine(RootDirectory, testFile.RelativeFilePath);
        }

        private bool HasSubDirectories()
        {
            return HasStringInRelativeTestFilePaths("/") || HasStringInRelativeTestFilePaths("\\");
        }

        private bool HasStringInRelativeTestFilePaths(string search)
        {
            return _testFiles.Any(file => file.RelativeFilePath.Contains(search));
        }

        private void CreateSubDirectories()
        {
            var filesInSubDirectories = FindAllFilesInSubDirectories();
            var directoryNames = ExtractDirectoryNames(filesInSubDirectories);

            foreach (var directoryName in directoryNames)
            {
                Directory.CreateDirectory(Path.Combine(RootDirectory, directoryName));
            }
        }

        private IEnumerable<string> FindAllFilesInSubDirectories()
        {
            return _testFiles
                .Select(file => file.RelativeFilePath)
                .Where(path => path.Contains("/") || path.Contains("\\"));
        }

        private static IEnumerable<string> ExtractDirectoryNames(IEnumerable<string> subfolders)
        {
            return subfolders.Select(ExtractSubDirectory);
        }

        private static string ExtractSubDirectory(string path)
        {
            var slash = path.LastIndexOf("/", StringComparison.Ordinal);
            var backslash = path.LastIndexOf("\\", StringComparison.Ordinal);

            if (slash > backslash)
            {
                return path.Substring(0, slash);
            }

            if (backslash > slash)
            {
                return path.Substring(0, backslash);
            }

            throw new ArgumentException("no subfolders");
        }
    }
}
