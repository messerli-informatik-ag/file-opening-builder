namespace Messerli.Test.Utility
{
    public class TestFile
    {
        public string SourceFilePath { get; }

        public string RelativeFilePath { get; }

        public TestFile(string filePath)
        {
            SourceFilePath = filePath;
            RelativeFilePath = filePath;
        }

        public TestFile(string sourceFilePath, string relativeFilePath)
        {
            SourceFilePath = sourceFilePath;
            RelativeFilePath = relativeFilePath;
        }
    }
}