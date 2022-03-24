using ImGuiNET;
namespace OpenTPW;

[FileHandler( @"\..*", IsDefault = true )]
public class GenericFileHandler : BaseFileHandler
{
	public GenericFileHandler( byte[] fileData ) : base( fileData ) { }

	public override void Draw()
	{
		base.Draw();
		ImGui.Text( $"File type: Generic (hex)" );

		ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
		ImGui.BeginChild( "ride_hex" );
		ImGui.PushFont( Editor.MonospaceFont );
		ImGui.Columns( 2, "ride_hex_columns", false );

		int minBytes = 0;
		int maxBytes = Math.Min( FileData.Length, minBytes + 512 );

		ImGuiListClipper clipperRef = new();
		ImGuiListClipperPtr clipper;
		unsafe
		{
			clipper = new ImGuiListClipperPtr( &clipperRef );
		}

		clipper.Begin( FileData.Length / 16 );
		clipper.ItemsHeight = 16;

		while ( clipper.Step() )
		{
			for ( int i = clipper.DisplayStart * 16; i < clipper.DisplayEnd * 16; i++ )
			{
				if ( i % 16 == 0 )
				{
					EditorHelpers.DrawColoredText( $"0x{i:X4}:", OneDark.Generic );
				}

				ImGui.SameLine();

				var b = FileData[i];
				var color = OneDark.Generic;
				if ( b == 0 )
					color = OneDark.DullGeneric;

				EditorHelpers.DrawColoredText( $"{b:X2}", color, align: false );
			}

			ImGui.NextColumn();

			for ( int i = clipper.DisplayStart * 16; i < clipper.DisplayEnd * 16; i++ )
			{
				if ( i % 16 == 0 )
				{
					ImGui.NewLine();
				}

				ImGui.SameLine();

				var b = FileData[i];
				char c = (char)b;

				var color = OneDark.Generic;
				if ( b < 32 )
				{
					c = '.';
					color = OneDark.DullGeneric;
				}

				EditorHelpers.DrawColoredText( $"{c:X1}", color, align: false );
			}

			ImGui.NextColumn();
		}

		ImGui.SetColumnWidth( 0, 450 );
		ImGui.Columns( 1 );
		ImGui.PopFont();
		ImGui.EndChild();
		ImGui.PopStyleColor();
	}
}
