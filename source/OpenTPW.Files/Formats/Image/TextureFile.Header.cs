namespace OpenTPW;

public partial class TextureFile
{
	public struct Header
	{
		public CompressionType CompressionType { get; set; }
		public byte Version { get; set; }
		public byte BitCount { get; set; }
		public byte Unknown0 { get; set; }

		public short Width { get; set; }
		public short Height { get; set; }
		public short YChannelQuantizationScale { get; set; }
		public short CbChannelQuantizationScale { get; set; }
		public short CrChannelQuantizationScale { get; set; }
		public short AChannelQuantizationScale { get; set; }

		public int BlockSize0 { get; set; }
		public int BlockSize1 { get; set; }

		public int Unknown1 { get; set; }

		public byte[] Block0 { get; set; }
		public byte[] Block1 { get; set; }

		public override string ToString()
		{
			var str = "";

			foreach ( var prop in typeof( Header ).GetProperties() )
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
