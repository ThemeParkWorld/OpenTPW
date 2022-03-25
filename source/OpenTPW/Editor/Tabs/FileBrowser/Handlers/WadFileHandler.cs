using ImGuiNET;

namespace OpenTPW;

[FileHandler( @"\.wad", icon: "content/icons/archive.png" )]
public class WadFileHandler : BaseFileHandler
{
	private WadArchive wadArchive;

	public WadFileHandler( byte[] fileData ) : base( fileData )
	{
		using var stream = new MemoryStream( fileData );
		wadArchive = new( stream );
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"File type: WAD archive" );
		ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );

		ImGui.BeginChild( "wad_contents" );

		if ( ImGui.BeginTable( "wad_contents", 3, ImGuiTableFlags.PadOuterX ) )
		{
			ImGui.TableNextColumn();
			ImGui.TableHeader( "Filename" );
			ImGui.TableNextColumn();
			ImGui.TableHeader( "Decomp. Size" );
			ImGui.TableNextColumn();
			ImGui.TableHeader( "Offset" );
			ImGui.TableNextRow();

			foreach ( var file in wadArchive.Files )
			{
				ImGui.TableNextColumn();
				ImGui.Text( $"{file.Name}" );
				ImGui.TableNextColumn();
				ImGui.Text( $"{file.DecompressedSize}" );
				ImGui.TableNextColumn();
				ImGui.Text( $"{file.ArchiveOffset}" );
			}

			ImGui.EndTable();
		}

		ImGui.EndChild();
		ImGui.PopFont();
		ImGui.PopStyleColor( 2 );
	}
}
