using ImGuiNET;

namespace OpenTPW;

[EditorMenu( "Scene/Outliner" )]
internal class SceneTab : BaseTab
{
	private Entity selectedEntity;

	public override void Draw()
	{
		ImGui.Begin( "Scene", ref visible );

		//
		// Hierarchy
		//
		{
			ImGui.SetNextItemWidth( -1 );
			ImGui.BeginListBox( "##hierarchy" );

			foreach ( var entity in Entity.All )
			{
				var selectableText = $"{entity.Name}\n" +
					$"({entity.GetType().Name})\n";

				if ( ImGui.Selectable( selectableText ) )
				{
					selectedEntity = entity;
				}

				ImGui.Separator();
			}

			ImGui.EndListBox();
		}

		ImGui.Separator();

		//
		// Inspector
		//
		{
			if ( selectedEntity != null )
			{
				if ( ImGui.BeginTable( $"##table_{selectedEntity.Name}", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.PadOuterX ) )
				{
					// TODO: Use reflection for this, combined with a [Scene.Show] attribute
					string[] names = new[] { "Position", "Rotation", "Scale" };
					string[] values = new[] { $"{selectedEntity.position}", $"{selectedEntity.rotation}", $"{selectedEntity.scale}" };

					ImGui.TableNextColumn();
					ImGui.TableHeader( "Name" );
					ImGui.TableNextColumn();
					ImGui.TableHeader( "Value" );
					ImGui.TableNextRow();

					for ( int i = 0; i < names.Length; i++ )
					{
						ImGui.TableNextColumn();
						ImGui.Text( $"{names[i]}" );
						ImGui.TableNextColumn();
						ImGui.Text( $"{values[i]}" );
					}

					ImGui.EndTable();
				}
			}
		}

		ImGui.End();
	}
}
