using ImGuiNET;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenTPW;

[EditorMenu( "Debug/File Tree" )]
internal class FileBrowserTab : BaseTab
{
	private string selectedDirectory = "data";

	/// <summary>
	/// Represents <see cref="selectedDirectory"/>, but clears and re-builds the current
	/// filesystem icon cache when set.
	/// </summary>
	private string SelectedDirectory
	{
		get => selectedDirectory;
		set
		{
			selectedDirectory = value;

			IconCache.Clear();
			CacheDirectory( selectedDirectory, selectedDirectory );
		}
	}

	/// <summary>
	/// Represents icons to display within the currently selected directory.
	/// </summary>
	private List<(Texture Icon, string Path, bool IsDirectory)> IconCache = new();

	private Texture FolderIcon { get; }
	private Texture ArchiveIcon { get; }

	private List<FilePreviewTab> SubTabs { get; } = new();

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
		CacheDirectory( SelectedDirectory, SelectedDirectory );

		FolderIcon = new Texture( "content/icons/folder.png" );
		ArchiveIcon = new Texture( "content/icons/archive.png" );
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
				var icon = new Texture( attribute.Icon );
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

		EditorHelpers.Image( icon ?? FolderIcon, IconSize );

		text = Path.GetFileName( text );

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
			col.A = 0.25f;

			var halfPadding = IconPadding / 2f;
			var rectOffset = new System.Numerics.Vector2( 0, -8 );

			drawList.AddRectFilled(
				pos - halfPadding + rectOffset,
				pos + IconSize + halfPadding + rectOffset,
				ImGui.GetColorU32( col ),
				8f,
				ImDrawFlags.None );
		}

		iconPosition.X += IconSize.X + IconPadding.X;
		var maxWidth = ImGui.GetWindowWidth() - IconSize.X;

		if ( iconPosition.X > maxWidth )
		{
			iconPosition.X = IconPadding.X;
			iconPosition.Y += IconSize.Y + IconPadding.Y;
		}
	}

	private void CacheDirectory( string rootPath, string directory )
	{
		foreach ( var subDir in FileSystem.Game.GetDirectories( directory ) )
		{
			var icon = subDir.EndsWith( ".wad" ) || subDir.EndsWith( ".sdt" ) ? ArchiveIcon : FolderIcon;
			IconCache.Add( (icon, subDir, true) );
		}

		foreach ( var file in FileSystem.Game.GetFiles( directory ) )
		{
			var fileHandler = FindFileHandler( Path.GetExtension( file ) );
			var icon = fileHandler.Icon;

			IconCache.Add( (icon, file, false) );
		}
	}

	private void DrawCurrentDirectory()
	{
		foreach ( var icon in IconCache.ToArray() )
		{
			DrawIcon( icon.Icon, icon.Path );

			if ( ImGui.IsItemClicked() )
			{
				if ( icon.IsDirectory )
				{
					// Move into the selected directory
					SelectedDirectory = Path.GetRelativePath( Settings.Default.GamePath, icon.Path );
				}
				else
				{
					// Open the registered file handler for the selected file
					var fileHandler = FindFileHandler( Path.GetExtension( icon.Path ) );
					var fileData = FileSystem.Game.ReadAllBytes( icon.Path );

					var args = new object[] { fileData };
					selectedFileHandler = Activator.CreateInstance( fileHandler.Type, args ) as BaseFileHandler;

					SubTabs.Add( new FilePreviewTab( fileData, selectedFileHandler ) );
				}
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

		//
		// Breadcrumbs
		//
		{
			string totalPath = "";
			foreach ( var directory in SelectedDirectory.Split( Path.DirectorySeparatorChar ) )
			{
				totalPath = Path.Join( totalPath, directory );

				if ( ImGui.Button( directory ) )
				{
					SelectedDirectory = totalPath;
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
			DrawCurrentDirectory();
		}

		ImGui.EndChild();
		ImGui.End();

		SubTabs.ForEach( x => x.Draw() );
	}
}
