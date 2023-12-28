namespace OpenTPW;

public partial class TextureFile
{
	public struct TextureFileData
	{
		public TextureFlags Flags { get; set; }
		public bool HasAlpha { get; set; }
		public byte BitsPerPixel { get; set; }
		public byte Version { get; set; }

		public short Width { get; set; }
		public short Height { get; set; }
		public short YChannelQuantizationScale { get; set; }
		public short CbChannelQuantizationScale { get; set; }
		public short CrChannelQuantizationScale { get; set; }
		public short AChannelQuantizationScale { get; set; }

		public int ColorBlockSize { get; set; }
		public int AlphaBlockSize { get; set; }

		public int Checksum { get; set; }

		public byte[] ColorBlock { get; set; }
		public byte[] AlphaBlock { get; set; }

		public override string ToString()
		{
			var str = "";

			foreach ( var prop in typeof( TextureFileData ).GetProperties() )
			{
				if ( prop.PropertyType == typeof( byte[] ) )
				{
					var propValue = prop?.GetValue( this );
					if ( propValue == null )
						str += $"{prop.Name}: null\n";
					else
						str += $"{prop.Name}: {BitConverter.ToString( propValue as byte[] )}\n";
				}
				else
					str += $"{prop.Name}: {prop.GetValue( this )}\n";
			}

			return str;
		}
	}
}
