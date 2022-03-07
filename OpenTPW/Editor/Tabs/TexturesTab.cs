using ImGuiNET;
using System;
using System.Linq;

namespace OpenTPW;

internal class TexturesTab : BaseTab
{
	public TexturesTab()
	{

	}


	private int selectedTexture;

	public override void Draw()
	{
		ImGui.Begin( "Textures" );

		var textureList = Asset.All.OfType<Texture>().ToList();
		var texturePaths = textureList.Select( texture => $"{texture.Id}:\t{texture.Path}" ).ToList();

		ImGui.Combo( "Texture", ref selectedTexture, texturePaths.ToArray(), texturePaths.Count );

		if ( selectedTexture > textureList.Count - 1 )
			selectedTexture = textureList.Count - 1;

		if ( selectedTexture < 0 )
			selectedTexture = 0;

		var texture = textureList[selectedTexture];

		var windowWidth = ImGui.GetWindowSize().X;
		float ratio = texture.Height / (float)texture.Width;

		if ( ratio is float.NaN )
			ratio = 1f;

		ImGui.Text( $"Texture selected: {texture.Id}, ratio: {ratio} (w: {texture.Width}, h: {texture.Height})" );
		ImGui.Image(
			(IntPtr)texture.Id,
			new System.Numerics.Vector2( windowWidth, windowWidth * ratio ),
			new System.Numerics.Vector2( 0, 1 ),
			new System.Numerics.Vector2( 1, 0 ) );

		ImGui.End();
	}
}
