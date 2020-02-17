using System;

namespace OpenTPW.Files.BFWD
{
    public interface IArchive : IDisposable
    {
        void LoadArchive(string path);
    }
}
