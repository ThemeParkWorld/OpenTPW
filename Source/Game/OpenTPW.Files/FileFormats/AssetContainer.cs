namespace OpenTPW.Files.FileFormats
{
    public class IAssetContainer { }

    public class AssetContainer<T> : IAssetContainer
    {
        public T Data { get; set; }
        public AssetContainer(T data)
        {
            Data = data;
        }
    }
}
