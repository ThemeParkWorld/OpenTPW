using ImGuiNET;

namespace OpenTPW;

[FileHandler( @"\.mp2", "content/icons/sound.png" )]
public class SoundFileHandler : BaseFileHandler
{
	private MP2Reader mp2Reader;
	private MP2File mp2File;

	public SoundFileHandler( byte[] fileData ) : base( fileData )
	{
		using var stream = new MP2Stream( fileData );
		mp2Reader = new MP2Reader( stream );
		mp2File = mp2Reader.GetFile( stream );
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"File type: MP2" );
		ImGui.Text( $"File Name: {mp2File.Name}" );
		ImGui.PushStyleColor( ImGuiCol.FrameBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );
		if ( ImGui.Button( "Play" ) )
		{
			AudioPlayer.PlaySound( new MemoryStream( mp2File.Data ) );
		}

		if ( ImGui.Button( "Stop" ) )
		{

		}

		ImGui.PopFont();
		ImGui.PopStyleColor( 2 );
	}
}
