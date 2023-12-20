using NAudio.Wave;
using System.IO;

namespace OpenTPW;
public static class AudioPlayer
{
	public static void PlaySound( Stream stream )
	{
		var audioStream = new StreamMediaFoundationReader( stream );
		var waveOut = new WaveOutEvent();

		waveOut.Init( audioStream );
		audioStream.Seek( 0, SeekOrigin.Begin );
		waveOut.Play();
	}
}
