using ImGuiNET;

namespace OpenTPW;

[EditorMenu( "Scene/Outliner" )]
internal class SceneTab : BaseTab
{
	public override void Draw()
	{
		ImGui.Begin( "Scene", ref visible );

		foreach ( var obj in Entity.All )
		{
			if ( ImGui.TreeNode( obj.Name ) )
			{
				ImGui.Text( $"P: {obj.position}" );
				ImGui.Text( $"R: {obj.rotation}" );
				ImGui.Text( $"S: {obj.scale}" );

				ImGui.TreePop();
			}
		}

		ImGui.End();
	}
}
