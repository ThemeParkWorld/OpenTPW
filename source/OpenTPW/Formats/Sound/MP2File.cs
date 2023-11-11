namespace OpenTPW;
public sealed class MP2File
{
	public string Name { get; set; }
	public int Header { get; set; }
	public byte[] SoundData { get; set; }
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



	public MP2File( int header, string name, byte[] soundData, int sampleRate, int resolution, int soundType, int samples, byte[] data )
	{
		this.Header = header;
		this.Name = name;
		this.SoundData = soundData;
		this.SampleRate = sampleRate;
		this.Resolution = resolution;
		this._SoundType = (SoundType)soundType;
		this.Samples = samples;
		this.Data = data;
	}
}
