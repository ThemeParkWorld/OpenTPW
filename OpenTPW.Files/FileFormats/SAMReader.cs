using Engine.Utils.DebugUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenTPW.Files.FileFormats
{
    public struct SAMPair
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class SAMReader : IAssetReader
    {
        public string[] Extensions => new[] { ".sam" };

        private bool IsWhiteSpace(char character)
        {
            return character == ' ' || character == '\t';
        }

        private bool IsNewLine(char character)
        {
            return character == '\n' || character == '\r';
        }

        public AbstractAsset LoadAsset(byte[] data)
        {
            using var memoryStream = new MemoryStream(data);
            using var binaryReader = new BinaryReader(memoryStream);

            bool inComment = false, inString = false;

            string wordBuffer = "";
            bool isKey = true;
            SAMPair lineBuffer = new SAMPair();
            List<SAMPair> fileBuffer = new List<SAMPair>();

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1)
            {
                var character = binaryReader.ReadChar();
                if (character == '#')
                {
                    // Comment - ignore until next newline
                    inComment = true;
                    continue;
                }
                else if (character == '"')
                {
                    // String - register all characters until the next '"'
                    inString = !inString;
                    continue;
                }

                if ((IsWhiteSpace(character) && !inString) || IsNewLine(character))
                {
                    if (inComment)
                    {
                        if (IsNewLine(character))
                        {
                            inComment = false;
                        }

                        wordBuffer = "";
                        lineBuffer = new SAMPair();
                        continue;
                    }

                    if (!string.IsNullOrEmpty(wordBuffer))
                    {
                        if (isKey)
                        {
                            if (lineBuffer.Key == null)
                            {
                                lineBuffer.Key = wordBuffer;
                                isKey = !isKey;
                            }
                        }
                        else
                        {
                            if (lineBuffer.Value == null)
                            {
                                lineBuffer.Value = wordBuffer;
                                isKey = !isKey;
                            }
                        }
                    }

                    if (IsNewLine(character))
                    {
                        if (lineBuffer.Key != null && lineBuffer.Value != null)
                            fileBuffer.Add(lineBuffer);
                        lineBuffer = new SAMPair();
                    }

                    wordBuffer = "";
                    continue;
                }

                wordBuffer += character;
            }

            ListAll(fileBuffer);

            return new AbstractAsset()
            {
                DataType = typeof(SAMPair),
                Data = fileBuffer.Cast<object>().ToList()
            };
        }

        void ListAll(List<SAMPair> fileBuffer)
        {
            foreach (var pair in fileBuffer)
            {
                Logging.Log($"{pair.Key}: {pair.Value}");
            }
        }
    }
}
