using ImGuiNET;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenTPW;

[EditorMenu( "Debug/File Tree" )]
internal class FileBrowserTab : BaseTab
{
	private byte[] selectedFileData = new byte[0];
	private string selectedFileExtension = "";
	private string searchBox = "";

	private List<RegisteredFileHandler> fileHandlers = new();

	struct RegisteredFileHandler
	{
		public Regex Regex { get; set; }
		public Type Type { get; set; }

		public RegisteredFileHandler( Regex regex, Type type )
		{
			Regex = regex;
			Type = type;
		}
	}

	private BaseFileHandler? selectedFileHandler;

	public FileBrowserTab()
	{
		RegisterFileHandlers();
	}

	private void RegisterFileHandlers()
	{
		var types = Assembly.GetExecutingAssembly().GetTypes()
			.Where( m => m.GetCustomAttribute<FileHandlerAttribute>() != null );

		void AddFileHandlers( bool matchDefault )
		{
			foreach ( var type in types )
			{
				var attribute = type.GetCustomAttribute<FileHandlerAttribute>();

				if ( attribute.IsDefault != matchDefault )
					continue;

				var regex = new Regex( attribute.RegexPattern );
				fileHandlers.Add( new( regex, type ) );
			}
		}

		AddFileHandlers( false );

		// Defer default handlers to ensure we don't match their regex too early
		AddFileHandlers( true );
	}

	private void DisplayDirectory( string rootPath, string directory )
	{
		var relativePath = Path.GetRelativePath( rootPath, directory );
		var dirName = Path.GetFileName( relativePath );

		if ( searchBox.Length > 0 )
			ImGui.SetNextItemOpen( true );

		if ( searchBox.Length == 0 && !ImGui.TreeNodeEx( dirName, ImGuiTreeNodeFlags.SpanFullWidth ) )
			return;

		foreach ( var subdir in Directory.GetDirectories( directory ) )
		{
			DisplayDirectory( rootPath, subdir );
		}

		foreach ( var file in Directory.GetFiles( directory ) )
		{
			if ( searchBox.Length > 0 && !file.Contains( searchBox ) )
				continue;

			ImGui.TreeNodeEx( Path.GetRelativePath( rootPath, file ),
				ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen );

			if ( ImGui.IsItemClicked() )
			{
				selectedFileData = File.ReadAllBytes( file );
				selectedFileExtension = Path.GetExtension( file );

				var fileHandler = fileHandlers.FirstOrDefault( h => h.Regex.Match( selectedFileExtension ).Success );

				var args = new object[] { selectedFileData };
				selectedFileHandler = Activator.CreateInstance( fileHandler.Type, args ) as BaseFileHandler;
			}
		}

		if ( searchBox.Length == 0 )
			ImGui.TreePop();
	}

	public override void Draw()
	{
		ImGui.Begin( "Files", ref visible );

		ImGui.Columns( 2 );

		ImGui.SetNextItemWidth( -1 );
		ImGui.InputText( "##search", ref searchBox, 256 );

		ImGui.BeginChild( "files" );

		//
		// Directory view
		//
		var path = GameDir.GetPath( "data/" );
		DisplayDirectory( path, path );

		ImGui.EndChild();

		//
		// File view
		//
		ImGui.NextColumn();
		ImGui.BeginChild( "hex_view" );

		selectedFileHandler?.Draw();

		ImGui.Columns( 1 );
		ImGui.End();
	}
}
