using System;
using System.Collections.Generic;

namespace OpenTPW.Files.FileFormats
{
    // STUPID STUPID STUPID!!!!
    public struct AbstractAsset 
    {
        public Type DataType { get; set; }
        public List<object> Data { get; set; }
    }
}
