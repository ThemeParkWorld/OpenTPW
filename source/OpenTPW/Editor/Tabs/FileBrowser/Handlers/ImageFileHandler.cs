using ImGuiNET;
using System.Numerics;

namespace OpenTPW;

[FileHandler( @"\.(png|tga|jpg|jpeg)", icon: "content/icons/image.png" )]
public class ImageFileHandler : BaseFileHandler
{
	private Texture texture;

	public ImageFileHandler( byte[] fileData ) : base( fileData )
	{
		using var fileStream = new MemoryStream( fileData );
		texture = new Texture( fileStream );
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"File type: Image" );

		var windowWidth = ImGui.GetWindowSize().X;
		float ratio = texture.Height / (float)texture.Width;

		if ( ratio is float.NaN )
			ratio = 1f;

		EditorHelpers.Image( texture, new Vector2( windowWidth, windowWidth * ratio ) );
	}
}
