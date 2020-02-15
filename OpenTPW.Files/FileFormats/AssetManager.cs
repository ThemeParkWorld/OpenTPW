using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenTPW.Files.FileFormats
{
    public static class AssetManager
    {
        public static Dictionary<string, IAssetReader> assetLoaders = new Dictionary<string, IAssetReader>();
        public static void Init()
        {
            // Automatically register all file formats via reflection for use later
            foreach (var assetReader_ in Assembly.GetExecutingAssembly().GetTypes().Where((type) => { return type.GetInterfaces().Contains(typeof(IAssetReader)); }))
            {
                IAssetReader assetReader = (IAssetReader)Activator.CreateInstance(assetReader_); // asset loader
                foreach (var extension in assetReader.extensions)
                {
                    assetLoaders.Add(extension, assetReader);
                }
            }
        }
    }
}
