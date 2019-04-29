using System.IO;

namespace Update.Client.InstallationGateway
{
    public interface IFileOpeningBuilder
    {
        IFileOpeningBuilder Create(bool create);

        IFileOpeningBuilder Truncate(bool truncate);

        IFileOpeningBuilder Append(bool append);

        IFileOpeningBuilder Write(bool write);

        IFileOpeningBuilder Read(bool read);

        IFileOpeningBuilder CreateNew(bool createNew);

        Stream Open(FileInfo path);
    }
}