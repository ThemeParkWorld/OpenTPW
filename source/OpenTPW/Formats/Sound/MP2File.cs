using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTPW;
public sealed class MP2File
{
	public string Name { get; set; }
	public int Header { get; set; }
	public byte[] Data { get; set; }
	public int SampleRate { get; set; }
	public int Resolution { get; set; }
	public enum SoundType
	{
		NONE = 0, //  on blanks
		WAV = 2, // on wav
		WAV_OLD = 3, // used before 1.7, like in the german cd version
		MP2_MONO = 36, // on mp2 (64kbit/s mono)
		MP2_STEREO = 37 // on mp2 (112kbit/s stereo)
	}
	public SoundType _SoundType;
	public int Samples { get; set; }

	public MP2File( string name, byte[] data, int sampleRate, int resolution, int soundType, int samples )
	{
		this.Name = name;
		this.Data = data;
		this.SampleRate = sampleRate;
		this.Resolution = resolution;
		this._SoundType = (SoundType)soundType;
		this.Samples = samples;
	}
	public MP2File( byte[] data )
	{
		this.Data = data;
	}

	public MemoryStream GetStream()
	{
		var stream = new MemoryStream( this.Data );
		stream.Seek( 0 , SeekOrigin.Begin ); // Reset position to 0 so we start from the beginning.

		return stream;
	}
}
