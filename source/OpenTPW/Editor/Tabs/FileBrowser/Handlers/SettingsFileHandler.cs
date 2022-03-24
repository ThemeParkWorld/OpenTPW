using ImGuiNET;

namespace OpenTPW;

[FileHandler( @"\.sam" )]
public class SettingsFileHandler : BaseFileHandler
{
	private SettingsFile settingsFile;

	public SettingsFileHandler( byte[] fileData ) : base( fileData )
	{
		using var stream = new MemoryStream( fileData );
		settingsFile = new SettingsFile( stream );
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"File type: Settings" );
		ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );

		ImGui.BeginChild( "sam_contents" );

		if ( ImGui.BeginTable( "sam_contents", 2, ImGuiTableFlags.PadOuterX ) )
		{
			ImGui.TableNextColumn();
			ImGui.TableHeader( "Key" );
			ImGui.TableNextColumn();
			ImGui.TableHeader( "Value" );
			ImGui.TableNextRow();

			foreach ( var item in settingsFile.Entries )
			{
				ImGui.TableNextColumn();
				ImGui.Text( $"{item.Item1}" );
				ImGui.TableNextColumn();
				ImGui.Text( $"{item.Item2}" );
			}

			ImGui.EndTable();
		}

		ImGui.EndChild();
		ImGui.PopFont();
		ImGui.PopStyleColor( 2 );
	}
}
