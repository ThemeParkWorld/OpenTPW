using ImGuiNET;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenTPW;

[EditorMenu( "Debug/File Tree" )]
internal class FileBrowserTab : BaseTab
{
	private byte[] selectedFileData = new byte[0];
	private string selectedFileExtension = "";
	private string searchBox = "";

	private List<(Regex, Type)> fileHandlers = new();

	private BaseFileManager? selectedFileHandler;

	public FileBrowserTab()
	{
		RegisterFileHandlers();
	}

	private void RegisterFileHandlers()
	{
		var types = Assembly.GetExecutingAssembly().GetTypes()
			.Where( m => m.GetCustomAttribute<FileManagerAttribute>() != null );

		void AddFileHandlers( bool matchDefault )
		{
			foreach ( var type in types )
			{
				var attribute = type.GetCustomAttribute<FileManagerAttribute>();

				if ( attribute.IsDefault != matchDefault )
					continue;

				var regex = new Regex( attribute.RegexPattern );
				fileHandlers.Add( (regex, type) );
			}
		}

		AddFileHandlers( false );
		AddFileHandlers( true );
	}

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
				var dirName = Path.GetFileName( relativePath );

				if ( searchBox.Length > 0 )
					ImGui.SetNextItemOpen( true );

				if ( searchBox.Length > 0 || ImGui.TreeNodeEx( dirName, ImGuiTreeNodeFlags.SpanFullWidth ) )
				{
					foreach ( var subdir in Directory.GetDirectories( directory ) )
					{
						DisplayDirectory( rootPath, subdir );
					}

					foreach ( var file in Directory.GetFiles( directory ) )
					{
						if ( searchBox.Length > 0 && !file.Contains( searchBox ) )
							continue;

						ImGui.TreeNodeEx( Path.GetRelativePath( rootPath, file ), ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen );

						if ( ImGui.IsItemClicked() )
						{
							selectedFileData = File.ReadAllBytes( file );
							selectedFileExtension = Path.GetExtension( file );

							var fileHandler = fileHandlers.FirstOrDefault( x => x.Item1.Match( selectedFileExtension ).Success );
							selectedFileHandler = Activator.CreateInstance( fileHandler.Item2, new object[] { selectedFileData } ) as BaseFileManager;
						}
					}

					if ( searchBox.Length == 0 )
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

		selectedFileHandler?.Draw();

		ImGui.Columns( 1 );
		ImGui.End();
	}
}
