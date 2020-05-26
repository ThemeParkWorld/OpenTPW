using Engine.ECS.Managers;
using OpenTPW.Files.FileFormats;
using OpenTPW.Files.FileFormats.BFWD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenTPW.Files
{
    public class FileManager : Manager<FileManager>
    {
        private Dictionary<string[], IAssetReader> assetReaders = new Dictionary<string[], IAssetReader>();

        public FileManager()
        { 
            RegisterAssetReaders();
        }

        private void RegisterAssetReaders()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IAssetReader)) && t != typeof(IAssetReader)))
            {
                var reader = (IAssetReader)Activator.CreateInstance(type);
                assetReaders.Add(reader.Extensions, reader);
            }
        }

        public void ReadFile(string assetArchivePath, string assetName)
        {
            var fileArchive = new BFWDArchive(assetArchivePath);

            var assetFile = fileArchive.files.First(file => file.name.Equals($"{assetName}\0", StringComparison.OrdinalIgnoreCase));
            var assetExtension = Path.GetExtension(assetName);
            var assetReader = assetReaders.First(r => r.Key.Contains(assetExtension));

            assetReader.Value.LoadAsset(assetFile.data);
        }
    }
}
