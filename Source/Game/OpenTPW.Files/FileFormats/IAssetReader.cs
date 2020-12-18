namespace OpenTPW.Files.FileFormats
{
    public interface IAssetReader
    {
        string AssetName { get; }
        string[] Extensions { get; }
        IAssetContainer LoadAsset(byte[] data);
    }
}
