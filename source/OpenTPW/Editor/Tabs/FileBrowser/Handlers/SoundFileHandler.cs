using ImGuiNET;
using OpenTPW;
using System.Media;
using NAudio;
using NAudio.Wave;

namespace OpenTPW;

[FileHandler( @"\.mp2" )]
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
		ImGui.Text( $"Sound Type: {mp2File._SoundType.ToString()}" );
		ImGui.PushStyleColor( ImGuiCol.FrameBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );
		if ( ImGui.Button( "Play" ) )
		{
			//using ( MemoryStream ms = new MemoryStream( data ) )
			//{
			//	using ( WaveStream wave = new BlockAlignReductionStream( WaveFormatConversionStream.CreatePcmStream( new Mp3FileReader( ms ) ) ) )
			//	{
			//		using ( WaveOutEvent waveOut = new WaveOutEvent() )
			//		{
			//			waveOut.Init( blockAlignedStream );
			//			waveOut.Play();
			//			while ( waveOut.PlaybackState == PlaybackState.Playing )
			//			{
			//				System.Threading.Thread.Sleep( 100 );
			//			}
			//		}
			//	}
			//}

			BufferedWaveProvider waveProvider = new BufferedWaveProvider( new WaveFormat( mp2File.SampleRate, 1 ) );
			using ( WaveOutEvent waveOut = new WaveOutEvent() )
			{
				waveOut.Init( waveProvider );
				waveOut.Play();
			}
		}


		if ( ImGui.Button( "Stop" ) )
		{
		}

		if( ImGui.Button( "Create File" ) )
		{
			System.IO.File.WriteAllBytes( $"content\\audio\\{mp2File.Name}", mp2File.Data );
		}

		ImGui.PopFont();
		ImGui.PopStyleColor( 2 );
	}
}
