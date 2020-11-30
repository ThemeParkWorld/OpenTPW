namespace Engine.Utils.FileUtils
{
    public class FileArchiveFile
    {
        public FileArchiveFile(string fileName, long fileLocation, long fileLength, CompressionMethod fileCompressionMethod, byte[] fileData)
        {
            FileName = fileName;
            FileLocation = fileLocation;
            FileLength = fileLength;
            FileCompressionMethod = fileCompressionMethod;
            FileData = fileData;
        }

        public string FileName { get; }
        public long FileLocation { get; }
        public long FileLength { get; }
        public CompressionMethod FileCompressionMethod { get; }
        public byte[] FileData { get; set; }
    }
}