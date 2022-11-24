using ImGuiNET;

namespace OpenTPW;

[FileHandler( @"\.wct", icon: "content/icons/image.png" )]
public class TextureFileHandler : BaseFileHandler
{
	private TextureFile textureFile;

	public TextureFileHandler( byte[] fileData ) : base( fileData )
	{
		using var fileStream = new MemoryStream( fileData );
		textureFile = new TextureFile( fileStream );
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"File type: Texture" );

		var windowWidth = ImGui.GetWindowSize().X;
		float ratio = textureFile.Texture.Height / (float)textureFile.Texture.Width;

		if ( ratio is float.NaN )
			ratio = 1f;

		ImGui.Text( textureFile.FileHeader.ToString() );

		EditorHelpers.Image( textureFile.Texture, new Vector2( windowWidth, windowWidth * ratio ) );
	}
}
