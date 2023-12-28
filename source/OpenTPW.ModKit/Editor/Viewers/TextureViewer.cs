using ImGuiNET;
using System.Runtime.InteropServices;
using Veldrid;

namespace OpenTPW.ModKit;

internal interface IFileViewer
{
	void DrawPreview();
	void DrawInfo();
}

[System.AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = false )]
internal sealed class HandlesExtensionAttribute : Attribute
{
	public readonly string Extension;

	public HandlesExtensionAttribute( string extension )
	{
		Extension = extension;
	}
}

[HandlesExtension( ".wct" )]
internal class TextureViewer : IFileViewer
{
	private TextureFile textureFile;

	private Texture texture;
	private TextureView textureView;
	private nint imguiPtr;

	public TextureViewer( string fileName )
	{
		textureFile = new TextureFile( fileName );
		CreateVeldridTexture();
	}
	private int CalculateMipLevels( int width, int height, int depth )
	{
		int maxDimension = Math.Max( width, Math.Max( height, depth ) );
		int mipLevels = (int)Math.Floor( Math.Log( maxDimension, 2 ) ) + 1;

		return mipLevels;
	}

	private void CreateVeldridTexture()
	{
		var t = textureFile.Data;
		var width = (uint)t.Width;
		var height = (uint)t.Height;
		var data = t.Data;
		var Device = Editor.Instance.graphicsDevice;
		var mipLevels = (uint)CalculateMipLevels( (int)width, (int)height, 1 );

		if ( width == 0 || height == 0 )
			return;

		var textureDescription = TextureDescription.Texture2D(
			width,
			height,
			mipLevels,
			1,
			PixelFormat.R8_G8_B8_A8_UNorm,
			TextureUsage.Sampled | TextureUsage.GenerateMipmaps
		);

		texture = Device.ResourceFactory.CreateTexture( textureDescription );

		var textureDataPtr = Marshal.AllocHGlobal( data.Length );
		Marshal.Copy( data, 0, textureDataPtr, data.Length );
		Device.UpdateTexture( texture, textureDataPtr, (uint)data.Length, 0, 0, 0, width, height, 1, 0, 0 );
		Marshal.FreeHGlobal( textureDataPtr );

		textureView = Device.ResourceFactory.CreateTextureView( texture );

		var commandList = Device.ResourceFactory.CreateCommandList();
		commandList.Begin();

		commandList.GenerateMipmaps( texture );

		commandList.End();
		Device.SubmitCommands( commandList );

		imguiPtr = ImGuiManager.GetOrCreateImGuiBinding( Device.ResourceFactory, textureView );
	}

	public void DrawPreview()
	{
		var Device = Editor.Instance.graphicsDevice;

		if ( imguiPtr == IntPtr.Zero )
		{
			ImGui.Text( $"No imguiPtr?" );
			return;
		}

		var size = ImGui.GetWindowWidth() - 16; /* Padding approx. */
		ImGui.Image( imguiPtr, new Vector2( size ) );
	}

	public void DrawInfo()
	{
		ImGui.Text( $"{textureFile.FileHeader}" );
	}
}
