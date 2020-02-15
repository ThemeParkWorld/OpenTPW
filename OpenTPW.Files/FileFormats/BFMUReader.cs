using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenTPW.Files.FileFormats
{
    // bullfrog multibyte <-> unicode?
    public class BFMUReader : IAssetReader
    {
        private List<char> _characters = new List<char>();

        public List<string> extensions => new List<string>() { ".dat" };

        public char GetChar(byte b)
        {
            return _characters[(int)b - 0x01]; // BFMU characters are offset by 0x01
        }

        public void LoadAsset(BFWDArchive archive, string file)
        {
            var fileMatches = archive.files.Where((bfwdFile) => bfwdFile.name == file);
            if (fileMatches.Count() < 1)
            {
                throw new Exception($"File {file} does not exist in archive.");
            }
            else
            {
                foreach (BFWDFile bfwdFile in fileMatches)
                {
                    LoadAsset(bfwdFile);
                    break; // Only load 1st file; ignore rest
                }
            }
        }

        public void LoadAsset(BFWDFile file)
        {
            LoadAsset(file.data);
        }

        public void LoadAsset(byte[] data)
        {
            MemoryStream memoryStream = new MemoryStream(data);
            BinaryReader binaryReader = new BinaryReader(memoryStream);

            if (Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) != "BFMU") throw new Exception("This isn't a BFMU file!");

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

            for (int i = 0; i < characterCount; ++i)
            {
                byte[] bytes = binaryReader.ReadBytes(2);
                foreach (char c in Encoding.Unicode.GetChars(bytes))
                {
                    _characters.Add(c);
                }
            }

            binaryReader.Close();
            memoryStream.Close();
        }

        public void LoadAsset(string file)
        {
            StreamReader streamReader = new StreamReader(file);
            byte[] buffer = new byte[streamReader.BaseStream.Length];
            streamReader.BaseStream.Read(buffer, 0, buffer.Length);
            LoadAsset(buffer);
            streamReader.Close();
        }

        public bool ReaderIsCompatible(string file)
        {
            throw new NotImplementedException();
        }

        public bool ReaderIsCompatible(BFWDFile file)
        {
            throw new NotImplementedException();
        }

        public bool ReaderIsCompatible(BFWDFile archive, string file)
        {
            throw new NotImplementedException();
        }
    }
}
