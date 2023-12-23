using NAudio.Wave;

namespace OpenTPW;

public static class Sound
{
	public static void Play( string path )
	{
		using var stream = FileSystem.OpenRead( path );

		var audioStream = new StreamMediaFoundationReader( stream );
		var waveOut = new WaveOutEvent();

		waveOut.Init( audioStream );
		audioStream.Seek( 0, SeekOrigin.Begin );
		waveOut.Play();
	}
}
