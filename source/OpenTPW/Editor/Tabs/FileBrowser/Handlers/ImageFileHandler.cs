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
		texture = TextureBuilder.UITexture.FromStream( fileStream ).Build();
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

		var startPos = ImGui.GetItemRectMin();

		var drawList = ImGui.GetWindowDrawList();
		var col = ImGui.GetColorU32( Vector4.One );

		if ( texture.Width > 128 || texture.Height > 128 )
			return;

		for ( int y = 1; y < texture.Height; y++ )
		{
			float lineX = 0f + startPos.X;
			float lineY = (((windowWidth * ratio) / (float)texture.Height) * y) + startPos.Y;

			drawList.AddLine( new System.Numerics.Vector2( lineX, lineY ), new System.Numerics.Vector2( startPos.X + windowWidth, lineY + 2 ), col );
		}

		for ( int x = 1; x < texture.Width; x++ )
		{
			float lineX = ((windowWidth * ratio / (float)texture.Width) * x) + startPos.X;
			float lineY = 0f + startPos.Y;

			drawList.AddLine( new System.Numerics.Vector2( lineX, lineY ), new System.Numerics.Vector2( lineX + 2, startPos.Y + (windowWidth * ratio) ), col );
		}
	}
}
