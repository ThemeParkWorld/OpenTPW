namespace Engine.Utils.FileUtils.FileSystems
{
    public interface IFileSystem
    {
        Asset GetAsset(string path);
        void Init(string contentPath);
    }
}
