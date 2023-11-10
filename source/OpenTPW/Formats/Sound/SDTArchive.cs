using OpenTPW;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenTPW;
public class SDTArchive
{
	private SDTStream memoryStream;
	public byte[] buffer;
	public List<MP2File> soundFiles;

	public SDTArchive( string path )
	{
		soundFiles = new List<MP2File>();
		using var fileStream = File.OpenRead( path );
		ReadFromStream( fileStream );
	}

	public SDTArchive( Stream stream )
	{
		soundFiles = new List<MP2File>();
		ReadFromStream( stream );
	}
	public void Dispose()
	{
		memoryStream.Dispose();
	}

	protected void ReadFromStream( Stream stream )
	{
		// Set up read buffer
		var tempStreamReader = new StreamReader( stream );
		var fileLength = (int)tempStreamReader.BaseStream.Length;

		buffer = new byte[fileLength];
		tempStreamReader.BaseStream.Read( buffer, 0, fileLength );
		tempStreamReader.Close();
		memoryStream = new SDTStream( buffer );

		ReadArchive();
	}

	public void ReadArchive(bool dataOnly = false)
	{
		memoryStream.Seek( 0, SeekOrigin.Begin );

		/*
			#File header
				4 bytes: File count
			#For each file
				4 bytes: File offset
			#For each file (at offset)
				4 bytes: Header size
				4 bytes: Data size
				16 bytes: File name (usually null terminated)
				4 bytes: Sample rate
				4 bytes: Resolution
				4 bytes: Sound type
					*See enum above (also: https://github.com/ufdada/dk2-tools/blob/6b4e49b607bbb7e0aa843856e584f6dd1365e7fc/Formats/Sound/sdt_struct.bt)
				4 bytes: Unknown
				4 bytes: Samples
				4 bytes: Unknown
				n bytes: File data
		*/

		var fileCount = memoryStream.ReadInt32();
		Log.Info( $"File Count: {fileCount}", true );
		Log.Info( $"", true );
		Log.Info( $"", true );


		List<int> offsets = new List<int>();

		// Gather Offsets
		for ( int i = 0; i < fileCount; i++ ) 
		{ 
			offsets.Add( memoryStream.ReadInt32() );
			Log.Info( $"Offset Found at: {offsets[i] }", true );
		}

		Log.Info( $"", true );
		Log.Info( $"", true );

		var listPosition = 0;
		foreach ( int offset in offsets )
		{
			Log.Info( $"Going to offset: {offset}" , true );
			memoryStream.Seek( offset , SeekOrigin.Begin );
			Log.Info( $"Seeked to memory position: {memoryStream.Position}", true );

			// Check if we only need to entire file data
			if ( !dataOnly ) 
			{
				var headerSize = memoryStream.ReadInt32();
				Log.Info( $"Header Size: {headerSize}", true );

				var dataSize = memoryStream.ReadInt32();
				Log.Info( $"Data Size: {dataSize}", true );

				var fileName = memoryStream.ReadString( 16 ).TrimEnd( '\0' );
				Log.Info( $"File Name: {fileName}", true );

				var sampleRate = memoryStream.ReadInt32();
				Log.Info( $"Sample Rate: {sampleRate}", true );

				var resolution = memoryStream.ReadInt32();
				Log.Info( $"Resolution: {resolution}", true );

				var soundType = memoryStream.ReadInt32();
				Log.Info( $"Sound/File Type: {soundType}", true );


				// Unknown
				_ = memoryStream.ReadInt32();

				var samples = memoryStream.ReadInt32();
				Log.Info( $"Samples: {samples}", true );
				Log.Info( $"", true );
				Log.Info( $"", true );

				// Unknown
				_ = memoryStream.ReadInt32();

				//Rest of Data
				var fileData = memoryStream.ReadBytes( dataSize );

				if ( fileName.Contains( ".mp2" ) )
				{
					MP2File soundFile = new MP2File( fileName, fileData, sampleRate, resolution, soundType, samples );
				
					soundFiles.Add( soundFile );
				}
			}
			else
			{
				int size;
				if ( offsets.Count < offsets[listPosition + 1] )
				{
					size = offsets[listPosition + 1] - offset;
				}
				else 
				{
					size = (int)(memoryStream.Length - memoryStream.Position);
				}
				
				var data = memoryStream.ReadBytes( size );
				MP2File soundFile = new MP2File( data );

				soundFiles.Add( soundFile );
			}
			listPosition++;
		}
	}

	public string[] GetFiles( string internalPath )
	{
		return soundFiles.Select( x => x.Name ).ToArray();
	}
	public string[] GetDirectories( string internalPath )
	{
		return soundFiles.Select( x => x.Name ).ToArray();
	}

	public MP2File GetFile( string name )
	{
		int index = soundFiles.FindIndex( x => x.Name.StartsWith( name ) );
		return soundFiles[index];
	}

}
