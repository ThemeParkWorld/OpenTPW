using ImGuiNET;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace OpenTPW;

[EditorMenu( "Debug/File Tree" )]
internal class FileBrowserTab : BaseTab
{
	private byte[] selectedFileData = new byte[0];
	private string selectedFileExtension = "";
	private string searchBox = "";
	private string selectedDirectory = "data/";

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

	private void DrawIcon( Texture icon, string text )
	{
		var xy = new System.Numerics.Vector2( x, y );
		ImGui.SetCursorPos( xy - new System.Numerics.Vector2( 0, 8 ) );

		ImGui.Image( (IntPtr)icon.Id, new System.Numerics.Vector2( 64, 64 ) );

		float mid = ImGui.CalcTextSize( text ).X;
		mid = 64 - mid;
		mid = mid / 2f;

		ImGui.SetCursorPos( xy + new System.Numerics.Vector2( mid, 64 - 8 ) );
		ImGui.Text( text );

		ImGui.SetCursorPos( xy );
		ImGui.InvisibleButton( $"button{text}", new System.Numerics.Vector2( 64, 64 ) );

		var drawList = ImGui.GetForegroundDrawList();

		if ( ImGui.IsItemHovered() )
		{
			var pos = ImGui.GetItemRectMin();
			var col = OneDark.Comment;
			col.W = 0.5f;

			var offset = new System.Numerics.Vector2( 16, 16 );
			drawList.AddRect( pos - offset, pos + new System.Numerics.Vector2( 64, 64 ) + offset, ImGui.GetColorU32( col ), 8f, ImDrawFlags.None, 4f );
		}

		x += 96;
		var maxWidth = ImGui.GetWindowWidth() - 48;
		if ( x > maxWidth )
		{
			x = 32;
			y += 96;
		}
	}

	float x = 0;
	float y = 0;

	private void DisplayDirectory( string rootPath, string directory )
	{
		var relativePath = Path.GetRelativePath( rootPath, directory );
		var dirName = Path.GetFileName( relativePath );

		if ( searchBox.Length > 0 )
			ImGui.SetNextItemOpen( true );


		foreach ( var subDir in Directory.GetDirectories( directory ) )
		{
			var subDirName = Path.GetFileName( subDir );

			if ( searchBox.Length == 0 )
			{
				DrawIcon( Icons.Folder, subDirName );

				if ( ImGui.IsItemClicked() )
				{
					selectedDirectory = Path.GetRelativePath( Settings.Default.GamePath, subDir );
				}
			}
			else
			{
				DisplayDirectory( rootPath, subDir );
			}
		}

		foreach ( var file in Directory.GetFiles( directory ) )
		{
			if ( searchBox.Length > 0 && !file.Contains( searchBox ) )
				continue;

			ImGui.SetCursorPos( new System.Numerics.Vector2( x, y ) );
			var icon = Icons.Document;

			switch ( Path.GetExtension( file ).ToLower() )
			{
				case ".wad":
					icon = Icons.Archive;
					break;
				case ".wct":
				case ".tga":
				case ".png":
				case ".jpg":
				case ".jpeg":
					icon = Icons.Image;
					break;
				case ".rse":
				case ".rss":
					icon = Icons.Ride;
					break;
				case ".sf2":
				case ".mp2":
				case ".sdt":
					icon = Icons.Sound;
					break;
				case ".md2":
					icon = Icons.Model;
					break;
				default:
					icon = Icons.Document;
					break;
			}

			DrawIcon( icon, Path.GetFileName( file ) );

			if ( ImGui.IsItemClicked() )
			{
				selectedFileData = File.ReadAllBytes( file );
				selectedFileExtension = Path.GetExtension( file );

				var fileHandler = fileHandlers.FirstOrDefault( h => h.Regex.Match( selectedFileExtension ).Success );

				var args = new object[] { selectedFileData };
				selectedFileHandler = Activator.CreateInstance( fileHandler.Type, args ) as BaseFileHandler;
			}
		}

		// if ( searchBox.Length == 0 )
		// ImGui.TreePop();
	}

	public override void Draw()
	{
		ImGui.Begin( "Files", ref visible );

		ImGui.Columns( 2 );

		ImGui.SetNextItemWidth( -1 );
		ImGui.InputText( "##search", ref searchBox, 256 );

		string totalPath = "";
		foreach ( var directory in selectedDirectory.Split( Path.DirectorySeparatorChar ) )
		{
			totalPath = Path.Join( totalPath, directory );

			if ( ImGui.Button( directory ) )
			{
				selectedDirectory = totalPath;
			}

			ImGui.SameLine();
			ImGui.Text( Path.DirectorySeparatorChar.ToString() );
			ImGui.SameLine();
		}

		ImGui.NewLine();

		ImGui.BeginChild( "files" );

		//
		// Directory view
		//
		var path = GameDir.GetPath( selectedDirectory );
		x = 32;
		y = 32;
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
