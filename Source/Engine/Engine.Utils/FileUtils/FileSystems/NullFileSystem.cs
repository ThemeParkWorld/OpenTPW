using Engine.Utils.DebugUtils;

namespace Engine.Utils.FileUtils.FileSystems
{
    class NullFileSystem : IFileSystem
    {
        public Asset GetAsset(string path)
        {
            return Asset.Empty;
        }

        public void Init(string contentPath)
        {
            Logging.Log("The null file system is in use; therefore, no files will be loaded.", Logging.Severity.Medium);
        }
    }
}
