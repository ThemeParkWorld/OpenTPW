using ImGuiNET;
using System.Text;
using System.Xml;

namespace OpenTPW;
[FileHandler( @"\.(TPWS|INTS|LAYS)" )]
public class SaveFileHandler : BaseFileHandler
{
	private SaveFileReader reader;
	private string text;

	public SaveFileHandler( byte[] fileData ) : base( fileData )
	{
		using var stream = new SaveFileStream( fileData );
		reader = new SaveFileReader( stream );
		text = reader.FileToString();
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"Save File Contents:" );
		ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );

		ImGui.InputTextMultiline(
			"##save_file_contents",
			ref text,
			(uint)text.Length,
			new System.Numerics.Vector2( -1, -1 ),
			ImGuiInputTextFlags.ReadOnly );

		ImGui.PopFont();
		ImGui.PopStyleColor( 2 );
	}

}
