using ImGuiNET;

namespace OpenTPW;

[EditorMenu( "Debug/File Tree" )]
internal class FileBrowserTab : BaseTab
{
	private byte[] selectedFileData = new byte[0];
	private string searchBox = "";

	public override void Draw()
	{
		ImGui.Begin( "Files", ref visible );

		ImGui.Columns( 2 );

		ImGui.SetNextItemWidth( -1 );
		ImGui.InputText( "##search", ref searchBox, 256 );

		ImGui.BeginChild( "files" );

		{
			void DisplayDirectory( string rootPath, string directory )
			{
				var relativePath = Path.GetRelativePath( rootPath, directory );

				if ( searchBox.Length > 0 )
					ImGui.SetNextItemOpen( true );

				if ( ImGui.TreeNodeEx( relativePath, ImGuiTreeNodeFlags.SpanFullWidth ) )
				{
					foreach ( var subdir in Directory.GetDirectories( directory ) )
					{
						DisplayDirectory( rootPath, subdir );
					}

					foreach ( var file in Directory.GetFiles( directory ) )
					{
						if ( searchBox.Length > 0 && !file.Contains( searchBox ) )
							continue;

						if ( ImGui.SmallButton( Path.GetRelativePath( rootPath, file ) ) )
						{
							selectedFileData = File.ReadAllBytes( file );
						}
					}

					ImGui.TreePop();
				}
			}

			var path = GameDir.GetPath( "data/" );

			DisplayDirectory( path, path );
		}

		ImGui.EndChild();

		//
		// Memory view
		//
		ImGui.NextColumn();
		ImGui.BeginChild( "hex_view" );

		{
			ImGui.PushStyleColor( ImGuiCol.ChildBg, OneDark.Background );
			ImGui.BeginChild( "ride_hex" );
			ImGui.PushFont( Editor.MonospaceFont );
			ImGui.Columns( 2, "ride_hex_columns", false );

			int minBytes = 0;
			int maxBytes = Math.Min( selectedFileData.Length, minBytes + 512 );

			ImGuiListClipper clipperRef = new();
			ImGuiListClipperPtr clipper;
			unsafe
			{
				clipper = new ImGuiListClipperPtr( &clipperRef );
			}

			clipper.Begin( selectedFileData.Length / 16 );
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

					var b = selectedFileData[i];
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

					var b = selectedFileData[i];
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

		ImGui.Columns( 1 );
		ImGui.End();
	}
}
