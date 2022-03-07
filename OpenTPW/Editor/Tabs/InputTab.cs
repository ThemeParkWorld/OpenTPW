using ImGuiNET;

namespace OpenTPW;
internal class InputTab : BaseTab
{
	public override void Draw()
	{
		ImGui.Begin( "Input" );

		ImGui.Text( $"Time: {Time.Now}" );

		ImGui.Text( $"Last keys down:" );
		Input.LastKeysDown.ForEach( key => ImGui.Text( $"\t{key}" ) );

		ImGui.Text( $"\n\nKeys down:" );
		Input.KeysDown.ForEach( key => ImGui.Text( $"\t{key}" ) );

		foreach ( var prop in typeof( Input ).GetProperties() )
		{
			ImGui.Text( $"{prop.Name}: {prop.GetValue( null )}" );
		}

		ImGui.End();
	}
}
