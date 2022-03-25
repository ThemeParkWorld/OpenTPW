using ImGuiNET;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenTPW;

[EditorMenu( "Debug/File Tree" )]
internal class FileBrowserTab : BaseTab
{
	private byte[] selectedFileData = new byte[0];
	private string searchBox = "";
	private string selectedDirectory = "data/";

	private Texture folderIcon;

	struct RegisteredFileHandler
	{
		public Texture Icon { get; set; }
		public Regex Regex { get; set; }
		public Type Type { get; set; }

		public RegisteredFileHandler( Regex regex, Type type, Texture icon )
		{
			Regex = regex;
			Type = type;
			Icon = icon;
		}
	}

	private List<RegisteredFileHandler> fileHandlers = new();

	private BaseFileHandler? selectedFileHandler;

	public FileBrowserTab()
	{
		RegisterFileHandlers();

		folderIcon = TextureBuilder.FromPath( "content/icons/folder.png", flipY: false ).Build();
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
				var icon = TextureBuilder.FromPath( attribute.Icon, flipY: false ).Build();
				var registeredFileHandler = new RegisteredFileHandler( regex, type, icon );

				fileHandlers.Add( registeredFileHandler );
			}
		}

		AddFileHandlers( false );

		// Defer default handlers to ensure we don't match their regex too early
		AddFileHandlers( true );
	}

	private System.Numerics.Vector2 iconPosition = new();
	private System.Numerics.Vector2 IconSize => new( 64, 64 );
	private System.Numerics.Vector2 IconPadding => new( 32, 32 );

	private void DrawIcon( Texture icon, string text )
	{
		ImGui.SetCursorPos( iconPosition - new System.Numerics.Vector2( 0, IconPadding.Y * 0.5f ) );
		ImGui.Image( (IntPtr)icon.Id, IconSize );

		//
		// Centered text offset
		//
		float mid = ImGui.CalcTextSize( text ).X;
		mid = (IconSize.X - mid) / 2f;

		ImGui.SetCursorPos( iconPosition + new System.Numerics.Vector2( mid, IconSize.Y - IconPadding.Y * 0.5f ) );
		ImGui.Text( text );

		ImGui.SetCursorPos( iconPosition );
		ImGui.InvisibleButton( $"button{text}", IconSize );

		var drawList = ImGui.GetForegroundDrawList();

		if ( ImGui.IsItemHovered() )
		{
			var pos = ImGui.GetItemRectMin();
			var col = OneDark.Comment;
			col.W = 0.5f;

			var halfPadding = IconPadding / 2f;
			var rectOffset = new System.Numerics.Vector2( 0, -8 );

			drawList.AddRect(
				pos - halfPadding + rectOffset,
				pos + IconSize + halfPadding + rectOffset,
				ImGui.GetColorU32( col ),
				8f,
				ImDrawFlags.None,
				4f );
		}

		iconPosition.X += IconSize.X + IconPadding.X;
		var maxWidth = ImGui.GetWindowWidth() - 48;
		if ( iconPosition.X > maxWidth )
		{
			iconPosition.X = IconPadding.X;
			iconPosition.Y += IconSize.Y + IconPadding.Y;
		}
	}

	private void DisplayDirectory( string rootPath, string directory )
	{
		foreach ( var subDir in Directory.GetDirectories( directory ) )
		{
			var subDirName = Path.GetFileName( subDir );

			if ( searchBox.Length == 0 )
			{
				DrawIcon( folderIcon, subDirName );

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

			var fileHandler = FindFileHandler( Path.GetExtension( file ) );
			var icon = fileHandler.Icon;

			DrawIcon( icon, Path.GetFileName( file ) );

			if ( ImGui.IsItemClicked() )
			{
				selectedFileData = File.ReadAllBytes( file );

				var args = new object[] { selectedFileData };
				selectedFileHandler = Activator.CreateInstance( fileHandler.Type, args ) as BaseFileHandler;
			}
		}
	}

	private RegisteredFileHandler FindFileHandler( string fileExtension )
	{
		return fileHandlers.FirstOrDefault( h => h.Regex.Match( fileExtension ).Success );
	}

	public override void Draw()
	{
		ImGui.Begin( "Files", ref visible );

		ImGui.Columns( 2 );

		//
		// Search box
		//
		{
			ImGui.SetNextItemWidth( -1 );
			ImGui.InputText( "##search", ref searchBox, 256 );
		}

		//
		// Breadcrumbs
		//
		{
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
		}

		ImGui.NewLine();

		//
		// Directory view
		//
		ImGui.BeginChild( "files" );
		{
			iconPosition = IconPadding;

			var path = GameDir.GetPath( selectedDirectory );
			DisplayDirectory( path, path );
		}
		ImGui.EndChild();

		//
		// File view
		//
		ImGui.NextColumn();
		ImGui.BeginChild( "file_view" );
		{
			selectedFileHandler?.Draw();
		}
		ImGui.EndChild();

		ImGui.Columns( 1 );
		ImGui.End();

	}
}
