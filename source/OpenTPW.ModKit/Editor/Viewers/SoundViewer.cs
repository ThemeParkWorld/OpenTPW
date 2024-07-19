using ImGuiNET;
using NAudio.Wave;
using Veldrid;

namespace OpenTPW.ModKit;

[HandlesExtension( ".mp2" )]
internal class SoundViewer : IFileViewer
{
	private SoundFile soundFile;

	public SoundViewer( string fileName )
	{
		soundFile = new SoundFile( fileName );
	}

	private void PlaySound()
	{
		using var stream = new MemoryStream( soundFile.buffer );
		var audioStream = new StreamMediaFoundationReader( stream );
		var waveOut = new WaveOutEvent();

		waveOut.Init( audioStream );
		audioStream.Seek( 0, SeekOrigin.Begin );
		waveOut.Play();
	}

	public void DrawPreview()
	{
		if ( ImGui.Button( "Play" ))
		{
			PlaySound();
		}
	}

	public TextureView GetIcon()
	{
		throw new NotImplementedException();
	}
}
