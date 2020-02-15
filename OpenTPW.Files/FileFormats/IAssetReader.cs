using System.Collections.Generic;

namespace OpenTPW.Files.FileFormats
{
    public interface IAssetReader
    {
        List<string> extensions { get; }
        void LoadAsset(byte[] data);
    }
}
