using ImGuiNET;

namespace OpenTPW;

[FileHandler( @"\.str", "content/icons/strings.png" )]
public class StringFileHandler : BaseFileHandler
{
	private StringFile stringFile;

	public StringFileHandler( byte[] fileData ) : base( fileData )
	{
		using var stream = new MemoryStream( fileData );
		stringFile = new StringFile( stream );
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"BFST File Contents:" );
		ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );

		foreach ( var entry in stringFile.Entries )
		{
			ImGui.Text( entry );
		}

		ImGui.PopFont();
		ImGui.PopStyleColor( 2 );
	}
}
