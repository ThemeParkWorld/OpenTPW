namespace OpenTPW.Files.FileFormats
{
    internal interface IAssetReader
    {
        string[] Extensions { get; }
        IAssetContainer LoadAsset(byte[] data);
    }
}
