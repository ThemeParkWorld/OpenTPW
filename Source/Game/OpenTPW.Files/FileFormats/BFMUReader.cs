using OpenTPW.Files.FileFormats.BFWD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenTPW.Files.FileFormats
{
    public class BFMUReader : IAssetReader
    {
        private List<char> characters = new List<char>();
        public string[] Extensions => new[] { ".dat" };

        public char GetChar(byte b)
        {
            return characters[(int)b - 0x01]; // BFMU characters are offset by 0x01
        }

        public IAssetContainer LoadAsset(ArchiveFile file)
        {
            return LoadAsset(file.data);
        }

        public IAssetContainer LoadAsset(byte[] data)
        {
            var memoryStream = new MemoryStream(data);
            var binaryReader = new BinaryReader(memoryStream);

            if (Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) != "BFMU")
                throw new Exception("This isn't a BFMU file!");

            /* 
             * BFMU (bullfrog multibyte <-> unicode) is like, the simplest format of all time
             * there are two BFMU files in theme park world:
             * MBtoUni.dat, UnitoMB.dat
             * these are used as 'indices' for each of the characters in the BFST files so that they can be displayed in-game.
             * this bfmureader aims to convert a byte (e.g. 0x0C) into its unicode counterpart (e.g. ' ').
             * 4 bytes - magic number - BFMU
             * 2 bytes - ?? - usually 0x00
             * 2 bytes - character count
             * for each character:
             *    - 2 bytes - the character itself in unicode form
             */

            binaryReader.ReadInt16();
            var characterCount = binaryReader.ReadInt16();

            for (var i = 0; i < characterCount; ++i)
            {
                var bytes = binaryReader.ReadBytes(2);
                foreach (var c in Encoding.Unicode.GetChars(bytes))
                {
                    characters.Add(c);
                }
            }

            binaryReader.Close();
            memoryStream.Close();

            return new AssetContainer<List<char>>(characters.ToList());
        }

        public void LoadAsset(string file)
        {
            var streamReader = new StreamReader(file);
            var buffer = new byte[streamReader.BaseStream.Length];
            streamReader.BaseStream.Read(buffer, 0, buffer.Length);
            LoadAsset(buffer);
            streamReader.Close();
        }

        public bool ReaderIsCompatible(string file)
        {
            throw new NotImplementedException();
        }

        public bool ReaderIsCompatible(ArchiveFile file)
        {
            throw new NotImplementedException();
        }

        public bool ReaderIsCompatible(ArchiveFile archive, string file)
        {
            throw new NotImplementedException();
        }
    }
}
