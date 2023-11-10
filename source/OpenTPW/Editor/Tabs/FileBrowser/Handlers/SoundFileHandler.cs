using ImGuiNET;
using OpenTPW.Formats.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using Vortice.Multimedia;

namespace OpenTPW;

[FileHandler( @"\.mp2" )]
public class SoundFileHandler : BaseFileHandler
{
	private MP2File mp2File;
	private SoundPlayer sound;

	public SoundFileHandler( byte[] fileData ) : base( fileData )
	{
		mp2File = new MP2File( fileData );
		sound = new SoundPlayer( mp2File.GetStream() );
	}

	public override void Draw()
	{

		base.Draw();

		ImGui.Text( $"File type: MP2" );
		ImGui.PushStyleColor( ImGuiCol.FrameBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );

		sound.Load();
		if ( ImGui.Button( "Play" ) )
		{
			if( sound.IsLoadCompleted ) 
			{ 
				sound.Play();
			}
		}

		if( ImGui.Button( "Stop" ) )
		{
			sound.Stop();
		}

		ImGui.PopFont();
		ImGui.PopStyleColor( 2 );
	}
}
