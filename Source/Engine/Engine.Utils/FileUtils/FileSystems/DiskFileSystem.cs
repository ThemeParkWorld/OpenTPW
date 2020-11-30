using Engine.Utils.DebugUtils;
using System;
using System.IO;

namespace Engine.Utils.FileUtils.FileSystems
{
    public class DiskFileSystem : IFileSystem
    {
        private string contentPath;

        public Asset GetAsset(string path)
        {
            var assetPath = path;

            if (!Path.IsPathRooted(path))
            {
                // Absolute path
                assetPath = $"{contentPath}/{path}";
            }

            if (!File.Exists(assetPath))
            {
                Logging.Log($"Asset {path} doesn't exist on disk.", Logging.Severity.High);
                return Asset.Empty;
            }

            var asset = new Asset(path, File.ReadAllBytes(assetPath));

            return asset;
        }

        public void Init(string contentPath)
        {
            this.contentPath = contentPath;
        }
    }
}
