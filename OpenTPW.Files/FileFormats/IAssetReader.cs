namespace OpenTPW.Files.FileFormats
{
    internal interface IAssetReader
    {
        string[] Extensions { get; }
        AbstractAsset LoadAsset(byte[] data);
    }
}
