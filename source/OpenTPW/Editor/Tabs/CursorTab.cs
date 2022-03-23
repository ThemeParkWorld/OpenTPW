using ImGuiNET;
using OpenTPW.UI;

namespace OpenTPW;

[EditorMenu( "Debug/Cursor" )]
internal class CursorTab : BaseTab
{
	public override void Draw()
	{
		ImGui.Begin( "Cursor", ref visible );

		var cursorTypes = Enum.GetValues( typeof( Input.CursorTypes ) ) as Input.CursorTypes[];
		var comboBoxItems = cursorTypes.Select( x => x.ToString() ).ToArray();
		var currentCursorType = (int)Cursor.Current.CursorType;

		ImGui.Combo( "Cursor type", ref currentCursorType, comboBoxItems, comboBoxItems.Length );

		if ( currentCursorType != (int)Cursor.Current.CursorType )
			Cursor.Current.CursorType = (Input.CursorTypes)currentCursorType;

		var texture = Cursor.Current.Texture;

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
