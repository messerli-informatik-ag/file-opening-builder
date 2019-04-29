using System;
using System.IO;

namespace Update.Client.InstallationGateway
{
    [Equals]
    public class FileOpeningBuilder : IFileOpeningBuilder
    {
        private bool _create;
        private bool _truncate;
        private bool _append;
        private bool _write;
        private bool _read;
        private bool _createNew;

        public IFileOpeningBuilder Create(bool create)
        {
            _create = create;
            return this;
        }

        public IFileOpeningBuilder Truncate(bool truncate)
        {
            _truncate = truncate;
            return this;
        }

        public IFileOpeningBuilder Append(bool append)
        {
            _append = append;
            return this;
        }

        public IFileOpeningBuilder Write(bool write)
        {
            _write = write;
            return this;
        }

        public IFileOpeningBuilder Read(bool read)
        {
            _read = read;
            return this;
        }

        public IFileOpeningBuilder CreateNew(bool createNew)
        {
            _createNew = createNew;
            return this;
        }

        public Stream Open(FileInfo path)
        {
            var settings = BuildSettings();
            HandleNotNativelySupportedConfigurations(path);
            return path.Open(settings.FileMode, settings.FileAccess, settings.FileShare);
        }

        private void HandleNotNativelySupportedConfigurations(FileInfo path)
        {
            if (_truncate && (_create || _createNew))
            {
                using (path.Create())
                {
                }
            }
        }

        private FileOpeningSettings BuildSettings()
        {
            var fileAccess =
                _read
                    ? _write
                        ? FileAccess.ReadWrite
                        : FileAccess.Read
                    : _write || _append
                        ? FileAccess.Write
                        : throw new InvalidOperationException(
                            "No file access has been specified." + Environment.NewLine +
                            "Specify at least one of the following accesses: read, write, append"
                        );

            var fileMode =
                _truncate
                    ? _append
                        ? throw new InvalidOperationException("Combining truncate and append makes no sense")
                        : _write
                            ? FileMode.Truncate
                            : throw new InvalidOperationException("Truncate requires write access")
                    : _createNew
                        ? _write || _append
                            ? FileMode.CreateNew
                            : throw new InvalidOperationException(
                                "CreateNew requires create or append access, but only had read access"
                            )
                        : _create
                            ? FileMode.OpenOrCreate
                            : _append
                                ? FileMode.Append
                                : FileMode.Open;

            const FileShare fileShare = FileShare.ReadWrite;

            return new FileOpeningSettings(fileMode, fileAccess, fileShare);
        }

        private struct FileOpeningSettings
        {
            public FileOpeningSettings(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
            {
                FileMode = fileMode;
                FileAccess = fileAccess;
                FileShare = fileShare;
            }

            public FileMode FileMode { get; }

            public FileAccess FileAccess { get; }

            public FileShare FileShare { get; }
        }
    }
}