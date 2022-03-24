using ImGuiNET;
using System.Text;

namespace OpenTPW;

[FileHandler( @"\.txt" )]
public class TextFileHandler : BaseFileHandler
{
	public TextFileHandler( byte[] fileData ) : base( fileData )
	{
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"File type: Text" );
		ImGui.PushStyleColor( ImGuiCol.FrameBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );

		var str = Encoding.ASCII.GetString( FileData );

		ImGui.InputTextMultiline(
			"##text_file_contents",
			ref str,
			(uint)str.Length,
			new System.Numerics.Vector2( -1, -1 ),
			ImGuiInputTextFlags.ReadOnly );

		ImGui.PopFont();
		ImGui.PopStyleColor( 2 );
	}
}
