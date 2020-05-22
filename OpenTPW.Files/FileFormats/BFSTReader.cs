using Engine.Utils.DebugUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenTPW.Files.FileFormats
{
    // bullfrog string
    public class BFSTReader
    {
        public List<string> strings = new List<string>();
        public BFSTReader(string path)
        {
            var characters = new BFMUReader();
            characters.LoadAsset(@"C:\Program Files (x86)\Bullfrog\Theme Park World\data\Language\English\MBToUni.dat");

            var streamReader = new StreamReader(path);
            var binaryReader = new BinaryReader(streamReader.BaseStream);
            if (Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) != "BFST") throw new Exception("not bfst");

            /* 
             * 4 bytes - magic number - BFST 
             * 4 bytes - ???
             * 4 bytes - string count
             * for each string:
             *    - 4 bytes - string offset (from end of string count bytes, so add 12)
             */

            binaryReader.ReadUInt32();
            var stringCount = binaryReader.ReadUInt32();

            var offsets = new List<uint>();
            for (var i = 0; i < stringCount; ++i)
                offsets.Add(binaryReader.ReadUInt32());

            /* 
             * for each string, we then have:
             * 1 byte - ALWAYS 0x01? maybe encoding
             * 3 bytes - length of string
             * n bytes - characters
             * each character comes from the BFMU file "MBToUni.dat"
             */

            foreach (int offset in offsets)
            {
                binaryReader.BaseStream.Seek(offset + 12, SeekOrigin.Begin);
                byte byte0;
                do
                {
                    byte0 = binaryReader.ReadByte();
                } while (byte0 != 0x01);
                var byte1 = binaryReader.ReadByte();
                var byte2 = binaryReader.ReadByte();
                var byte3 = binaryReader.ReadByte();
                var stringLength = ((uint)byte3 << 16 | (uint)byte2 << 8 | (uint)byte1);
                var str = "";
                for (var i = 0; i < stringLength; ++i)
                {
                    var b = binaryReader.ReadByte();

                    str += characters.GetChar(b);
                }
                binaryReader.ReadInt32();
                strings.Add(str);
            }

            binaryReader.Close();
            streamReader.Close();
        }

        public void WriteAllStrings()
        {
            for (var i = 0; i < strings.Count; ++i)
            {
                Logging.Log($"{strings[i]}");
            }
        }
    }
}
