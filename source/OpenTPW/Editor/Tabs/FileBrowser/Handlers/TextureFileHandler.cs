using ImGuiNET;
using System.Numerics;

namespace OpenTPW;

[FileHandler( @"\.wct", icon: "content/icons/image.png" )]
public class TextureFileHandler : BaseFileHandler
{
	private Texture texture;

	public TextureFileHandler( byte[] fileData ) : base( fileData )
	{
		using var fileStream = new MemoryStream( fileData );
		texture = new TextureFile( fileStream ).Texture;
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"File type: Texture" );

		var windowWidth = ImGui.GetWindowSize().X;
		float ratio = texture.Height / (float)texture.Width;

		if ( ratio is float.NaN )
			ratio = 1f;

		EditorHelpers.Image( texture, new Vector2( windowWidth, windowWidth * ratio ) );

		var startPos = ImGui.GetItemRectMin();

		var drawList = ImGui.GetWindowDrawList();
		var col = ImGui.GetColorU32( Vector4.One );
	}
}
