using StbImageSharp;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Veldrid;

namespace OpenTPW;

public partial class Texture : Asset
{
	public uint Width { get; private set; }
	public uint Height { get; private set; }

	internal Veldrid.Texture NativeTexture;
	internal TextureView NativeTextureView;

	/// <summary>
	/// From path on disk
	/// </summary>
	public Texture( string path )
	{
		// shit-tier hack
		StbImage.stbi_set_flip_vertically_on_load( 1 );

		var fileData = File.ReadAllBytes( path );
		var image = ImageResult.FromMemory( fileData, ColorComponents.RedGreenBlueAlpha );

		StbImage.stbi_set_flip_vertically_on_load( 0 );

		var data = image.Data;
		var width = (uint)image.Width;
		var height = (uint)image.Height;

		CreateTexture( path, data, width, height );
	}

	/// <summary>
	/// From game resource
	/// </summary>
	public Texture( TextureFile textureFile )
	{
		var textureData = textureFile.Data;
		CreateTexture( "", textureData.Data, (uint)textureData.Width, (uint)textureData.Height );
	}

	/// <summary>
	/// From data as bytes
	/// </summary>
	public Texture( byte[] data, int width, int height )
	{
		CreateTexture( "", data, (uint)width, (uint)height );
	}

	public Texture( Stream stream )
	{
		// shit-tier hack
		StbImage.stbi_set_flip_vertically_on_load( 1 );

		var fileData = new byte[stream.Length];
		stream.Read( fileData, 0, fileData.Length );

		var image = ImageResult.FromMemory( fileData, ColorComponents.RedGreenBlueAlpha );

		StbImage.stbi_set_flip_vertically_on_load( 0 );

		var data = image.Data;
		var width = (uint)image.Width;
		var height = (uint)image.Height;
		var debugName = $"Stream {stream.GetHashCode()}";

		CreateTexture( debugName, data, width, height );
	}

	private int CalculateMipLevels( int width, int height, int depth )
	{
		int maxDimension = Math.Max( width, Math.Max( height, depth ) );
		int mipLevels = (int)Math.Floor( Math.Log( maxDimension, 2 ) ) + 1;

		return mipLevels;
	}

	private void CreateTexture( string debugName, byte[] data, uint width, uint height )
	{
		if ( TryGetCachedTexture( debugName, out var cachedTexture ) )
		{
			NativeTexture = cachedTexture!.NativeTexture;
			NativeTextureView = cachedTexture!.NativeTextureView;

			return;
		}

		uint mipLevels = (uint)CalculateMipLevels( (int)width, (int)height, 1 );

		var textureDescription = TextureDescription.Texture2D(
			width,
			height,
			mipLevels,
			1,
			PixelFormat.R8_G8_B8_A8_UNorm,
			TextureUsage.Sampled | TextureUsage.GenerateMipmaps
		);

		var texture = Device.ResourceFactory.CreateTexture( textureDescription );

		var textureDataPtr = Marshal.AllocHGlobal( data.Length );
		Marshal.Copy( data, 0, textureDataPtr, data.Length );
		Device.UpdateTexture( texture, textureDataPtr, (uint)data.Length, 0, 0, 0, width, height, 1, 0, 0 );
		Marshal.FreeHGlobal( textureDataPtr );

		var textureView = Device.ResourceFactory.CreateTextureView( texture );

		Path = debugName;
		NativeTexture = texture;
		NativeTextureView = textureView;
		Width = width;
		Height = height;

		Game.renderer.ImmediateSubmit( cmd =>
		{
			cmd.GenerateMipmaps( NativeTexture );
		} );

		All.Add( this );
	}
}
