using System;

namespace OpenTPW.Files.FileFormats.BFWD
{
    public interface IArchive : IDisposable
    {
        void LoadArchive(string path);
    }
}
