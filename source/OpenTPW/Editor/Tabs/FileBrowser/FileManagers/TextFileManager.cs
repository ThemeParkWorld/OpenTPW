using ImGuiNET;
using System.Text;

namespace OpenTPW;

partial class FileManagers
{
	[FileManager( @"\.(sam|txt)" )]
	public static void DisplayTextFile( byte[] selectedFileData )
	{
		ImGui.Text( $"File type: Text" );
		ImGui.PushStyleColor( ImGuiCol.FrameBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );

		var str = Encoding.ASCII.GetString( selectedFileData );

		ImGui.InputTextMultiline(
			"##text_file_contents",
			ref str,
			(uint)str.Length,
			new System.Numerics.Vector2( -1, -1 ),
			ImGuiInputTextFlags.ReadOnly );

		ImGui.PopFont();
		ImGui.PopStyleColor(2);
	}
}
