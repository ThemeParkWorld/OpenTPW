using ImGuiNET;
using System.Text;

namespace OpenTPW;

[FileHandler( @"\.map" )]
public class MapFileHandler : BaseFileHandler
{
	public MapFileHandler( byte[] fileData ) : base( fileData )
	{
	}

	public byte GetRandomByte( int seed )
	{
		unchecked
		{
			int hash = seed.GetHashCode();
			return (byte)(hash % 255);
		}
	}

	public System.Numerics.Vector4 GetRandomColor( int seed )
	{
		switch ( seed )
		{
			case 0:
				return new System.Numerics.Vector4( 0, 0, 0, 1 );
			case 1:
				return System.Numerics.Vector4.One;
			case 2:
				return new System.Numerics.Vector4( 0.1f, 0.5f, 0.4f, 1 );
			case 3:
				return new System.Numerics.Vector4( 1.0f, 0.5f, 1.0f, 1 );
			case 17:
				return new System.Numerics.Vector4( 0, 1, 1, 1 );
			case 144:
				return new System.Numerics.Vector4( 1, 1, 0, 1 );

			default:
				return new System.Numerics.Vector4( 1, 0, 1, 1 );
		}
	}

	public override void Draw()
	{
		base.Draw();

		ImGui.Text( $"File type: Map" );
		ImGui.PushStyleColor( ImGuiCol.FrameBg, OneDark.Background );
		ImGui.PushStyleColor( ImGuiCol.Text, OneDark.Generic );
		ImGui.PushFont( Editor.MonospaceFont );

		var str = Encoding.ASCII.GetString( FileData );

		var dataStart = 0x48;
		var dataEnd = 0x4047;

		var data = FileData[dataStart..dataEnd];

		int i = 0;

		ImGui.BeginChild( "inner", new( -1, -1 ), true, ImGuiWindowFlags.AlwaysHorizontalScrollbar );
		foreach ( var b in data )
		{
			ImGui.PushStyleColor( ImGuiCol.Text, GetRandomColor( b ) );
			ImGui.Text( "-" );
			ImGui.PopStyleColor( 1 );

			i++;

			if ( i == 128 )
			{
				i = 0;
			}
			else
			{
				ImGui.SameLine();
			}
		}
		ImGui.EndChild();

		ImGui.PopFont();
		ImGui.PopStyleColor( 2 );
	}
}
