using System;
using System.Collections.Generic;

namespace OpenTPW.Files.FileFormats
{
    public class IAssetContainer 
    { 
        public object Data { get; set; }
    }

    public class AssetContainer<T> : IAssetContainer
    {
        public new T Data { get; set; }
        public AssetContainer(T data)
        {
            Data = data;
        }
    }
}
