using ImGuiNET;

namespace OpenTPW;

internal static class EditorHelpers
{
	public static string Align( string str ) => str.PadRight( 16, ' ' );

	public static void DrawColoredText( string str, System.Numerics.Vector4 col, bool align = true )
	{
		ImGui.PushStyleColor( ImGuiCol.Text, col );

		if ( align )
			str = Align( str );
		ImGui.Text( str );

		ImGui.PopStyleColor();
	}

	public static void ApplyPadding()
	{
		var padding = new System.Numerics.Vector2( 4, 2 );
		ImGui.SetCursorPos( ImGui.GetCursorPos() + padding );
	}

	public static void Image( Texture texture, Vector2 size )
	{
		var texPtr = Editor.Instance.ImGuiRenderer.GetOrCreateImGuiBinding( Device.ResourceFactory, texture.NativeTextureView );

		ImGui.Image( texPtr, size,
			new System.Numerics.Vector2( 0, 1 ), new System.Numerics.Vector2( 1, 0 ) );
	}
}
