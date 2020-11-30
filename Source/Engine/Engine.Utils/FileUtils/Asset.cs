using System;
using System.Text;

namespace Engine.Utils.FileUtils
{
    public class Asset
    {
        public string MountPath { get; }
        public byte[] Data { get; set; }

        public Asset(string mountPath, byte[] data)
        {
            MountPath = mountPath;
            Data = data;
        }

        public Asset() { }

        public static Asset Empty = new Asset();

        public string AsString() => Encoding.Default.GetString(Data);
    }
}