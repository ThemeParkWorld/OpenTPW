namespace OpenTPW;

public class WctFile
{
	public Texture? Texture { get; set; }

	struct FileInfo
	{
		public int Version { get; set; }
		public bool HasAlpha { get; set; }
		public int BitsPerPixel { get; set; }
		public int Compression { get; set; }
		public int ImageWidth { get; set; }
		public int ImageHeight { get; set; }

		public int FirstSectionLength { get; set; }
		public int SecondSectionLength { get; set; }
		public byte[] FirstSection { get; set; }
		public byte[] SecondSection { get; set; }


		public override string ToString()
		{
			var str = "";

			foreach ( var prop in typeof( FileInfo ).GetProperties() )
			{
				if ( prop.PropertyType == typeof( byte[] ) )
					str += $"{prop.Name}: {BitConverter.ToString( prop.GetValue( this ) as byte[] )}\n";
				else
					str += $"{prop.Name}: {prop.GetValue( this )}\n";
			}

			return str;
		}
	}

	public WctFile( string path )
	{
		using var fileStream = File.OpenRead( path );
		using var binaryReader = new BinaryReader( fileStream );

		FileInfo fileInfo = new();

		fileInfo.Version = binaryReader.ReadByte();

		fileInfo.HasAlpha = binaryReader.ReadBoolean();

		fileInfo.BitsPerPixel = binaryReader.ReadByte();

		fileInfo.Compression = binaryReader.ReadByte();

		fileInfo.ImageWidth = binaryReader.ReadInt16();
		fileInfo.ImageHeight = binaryReader.ReadInt16();

		_ = binaryReader.ReadInt16();
		_ = binaryReader.ReadInt16();
		_ = binaryReader.ReadInt16();
		_ = binaryReader.ReadInt16();

		fileInfo.FirstSectionLength = binaryReader.ReadInt32();

		fileInfo.SecondSectionLength = binaryReader.ReadInt32();

		fileInfo.FirstSection = binaryReader.ReadBytes( fileInfo.FirstSectionLength );

		_ = binaryReader.ReadInt16();

		fileInfo.SecondSection = binaryReader.ReadBytes( fileInfo.SecondSectionLength );

		Log.Trace( fileInfo );
	}
}
