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

    // TODO: Read hoarding & shape data
    public class SAMReader : IAssetReader
    {
        public string[] Extensions => new[] { ".sam" };

        private bool IsWhiteSpace(char character) => character == ' ' || character == '\t';

        private bool IsNewLine(char character) => character == '\n' || character == '\r';
        public string AssetName => "SAM (Settings and Modifiers)";

        public IAssetContainer LoadAsset(byte[] data)
        {
            using var memoryStream = new MemoryStream(data);
            using var binaryReader = new BinaryReader(memoryStream);

            var inComment = false;
            var inString = false;
            var isKey = true;

            var wordBuffer = "";
            var lineBuffer = new SAMPair();
            var samPairs = new List<SAMPair>();

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
                            samPairs.Add(lineBuffer);
                        lineBuffer = new SAMPair();
                    }

                    wordBuffer = "";
                    continue;
                }

                wordBuffer += character;
            }

            return new AssetContainer<List<SAMPair>>(samPairs.ToList());
        }
    }
}
