namespace OpenTPW.Files;

public sealed class ModelFile : BaseFormat
{
	public ModelFileData Data;

	public ModelFile( Stream stream )
	{
		ReadFromStream( stream );
	}

	public ModelFile( string path )
	{
		ReadFromFile( path );	
	}
	protected override void ReadFromStream( Stream stream )
	{
		using var binaryReader = new BinaryReader( stream );

		var magicNumber = binaryReader.ReadUInt32();
		var verMajor = binaryReader.ReadUInt32();
		var verMinor = binaryReader.ReadUInt32();

		stream.Seek( 0x17, SeekOrigin.Begin );
		var fileNameSizeStr = new string( binaryReader.ReadChar(), 1 );
		var fileNameSize = int.Parse( fileNameSizeStr ) + 4; /* Name + extension */
		var fileName = new string( binaryReader.ReadChars( fileNameSize ) );

		stream.Seek( 0x50, SeekOrigin.Begin );
		var textureDirectoryStart = binaryReader.ReadUInt32();
		var textureDirectoryEnd = binaryReader.ReadUInt32();

		var textureCount = (textureDirectoryEnd - textureDirectoryStart) / 20;

		stream.Seek( 24, SeekOrigin.Current );
		var meshesStart = binaryReader.ReadUInt32();

		// Textures
		List<string> textures = new();

		for ( int i = 0; i < textureCount; ++i )
		{
			stream.Seek( textureDirectoryStart + (i * 20), SeekOrigin.Begin );
			var texture = new string( binaryReader.ReadChars( 20 ) );
			textures.Add( texture );
		}

		// Mesh data
		stream.Seek( 0x208, SeekOrigin.Begin );
		var meshNameOffset = binaryReader.ReadUInt32();
		binaryReader.ReadUInt16();
		binaryReader.ReadUInt16();
		binaryReader.ReadUInt16();
		binaryReader.ReadUInt16();
		var posValueTableOffset = binaryReader.ReadUInt32();
		binaryReader.ReadUInt32();
		var uvValueTableOffset = binaryReader.ReadUInt32();
		var xIndexOffset = binaryReader.ReadUInt32();
		var yIndexOffset = binaryReader.ReadUInt32();
		binaryReader.ReadUInt32();

		var xPositionValueCount = 4;
		var yPositionValueCount = 4;
		var zPositionValueCount = 4;
		var uPositionValueCount = 4;
		var vPositionValueCount = 4;

		List<float> xValues = new();
		for ( int i = 0; i < xPositionValueCount; ++i )
		{
			stream.Seek( posValueTableOffset + (i * 4), SeekOrigin.Begin );
			xValues.Add( binaryReader.ReadSingle() );
		}

		List<float> yValues = new();
		for ( int i = 0; i < yPositionValueCount; ++i )
		{
			stream.Seek( posValueTableOffset + (i * 4) + (4 * 4), SeekOrigin.Begin );
			yValues.Add( binaryReader.ReadSingle() );
		}

		List<float> zValues = new();
		for ( int i = 0; i < zPositionValueCount; ++i )
		{
			stream.Seek( posValueTableOffset + (i * 4) + (4 * 4 * 2), SeekOrigin.Begin );
			zValues.Add( binaryReader.ReadSingle() );
		}

		List<float> uValues = new();
		for ( int i = 0; i < uPositionValueCount; ++i )
		{
			stream.Seek( uvValueTableOffset + (i * 4), SeekOrigin.Begin );
			uValues.Add( binaryReader.ReadSingle() );
		}

		List<float> vValues = new();
		for ( int i = 0; i < vPositionValueCount; ++i )
		{
			stream.Seek( uvValueTableOffset + (i * 4) + (4 * 4), SeekOrigin.Begin );
			vValues.Add( binaryReader.ReadSingle() );
		}

		List<int> indices = new();

		stream.Seek( yIndexOffset, SeekOrigin.Begin );
		var faceCount = 2;

		for ( int i = 0; i < faceCount; i++ )
		{
			stream.Seek( yIndexOffset + (i * 14) + 2, SeekOrigin.Begin );
			indices.Add( binaryReader.ReadInt16() );
			indices.Add( binaryReader.ReadInt16() );
			indices.Add( binaryReader.ReadInt16() );
		}

		//
		// Dump data
		//
		Log.Info( $"Magic number: {magicNumber:X}" );
		Log.Info( $"Version major: {verMajor:X}" );
		Log.Info( $"Version minor: {verMinor:X}" );
		Log.Info( $"File name: {fileName:X}" );
		Log.Info( $"Texture directory start: {textureDirectoryStart:X}" );
		Log.Info( $"Texture directory end: {textureDirectoryEnd:X}" );
		Log.Info( $"Meshes start: {meshesStart:X}" );
		Log.Info("");
		Log.Info( $"Texture count: {textureCount}" );
		Log.Info( $"Textures: {string.Join( ", ", textures )}" );
		Log.Info("");
		Log.Info( $"Mesh name offset: {meshNameOffset:X}" );
		Log.Info( $"Position value table offset: {posValueTableOffset:X}" );
		Log.Info( $"UV value table offset: {uvValueTableOffset:X}" );
		Log.Info( $"Position index offset: {yIndexOffset:X}" );
		Log.Info( $"Index table offset: {xIndexOffset:X}" );
		Log.Info("");
		Log.Info( $"X Positions: {string.Join( ", ", xValues )}" );
		Log.Info( $"Y Positions: {string.Join( ", ", yValues )}" );
		Log.Info( $"Z Positions: {string.Join( ", ", zValues )}" );
		Log.Info("");
		Log.Info( $"U Positions: {string.Join( ", ", uValues )}" );
		Log.Info( $"V Positions: {string.Join( ", ", vValues )}" );
		Log.Info("");
		Log.Info( $"Face count: {faceCount}" );
		Log.Info( $"Indices: {string.Join( ", ", indices )}" );

		Data = new();

		for ( int i = 0; i < xValues.Count; ++i )
		{
			Data.Vertices.Add( new Vertex()
			{
				Position = new Vector3( xValues[i], yValues[i], zValues[i] ),
				TexCoords = new Vector2( uValues[i], vValues[i] )
			} );
		}

		for ( int i = 0; i < indices.Count; ++i )
		{
			Data.Indices.Add( (uint)indices[i] );
		}
	}

	public struct Vertex
	{
		public Vector3 Position = new();
		public Vector2 TexCoords = new();

		public Vertex()
		{

		}
	}

	public struct ModelFileData
	{
		public List<Vertex> Vertices = new();
		public List<uint> Indices = new();

		public ModelFileData()
		{
			
		}
	}
}
