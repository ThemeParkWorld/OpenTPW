using ImGuiNET;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenTPW;

[EditorMenu( "Debug/File Tree" )]
internal class FileBrowserTab : BaseTab
{
	private string searchBoxText = "";
	private string selectedDirectory = "data";

	private Texture folderIcon;

	private List<FilePreviewTab> subTabs = new();

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

		var texPtr = ImGuiRenderer.GetOrCreateImGuiBinding( Device.ResourceFactory, icon.VeldridTextureView );
		ImGui.Image( texPtr, IconSize );

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
			col.W = 0.25f;

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

	private void StartProcess( string processName, string arguments )
	{
		var process = new Process();
		process.StartInfo.FileName = processName;
		process.StartInfo.Arguments = arguments;

		Log.Trace( arguments );
		process.Start();
	}

	private void DisplayDirectory( string rootPath, string directory )
	{
		foreach ( var subDir in Directory.GetDirectories( directory ) )
		{
			var subDirName = Path.GetFileName( subDir );

			if ( searchBoxText.Length == 0 )
			{
				DrawIcon( folderIcon, subDirName );

				//
				// Context menu
				//
				if ( ImGui.BeginPopupContextItem() )
				{
					ImGui.Text( $"Folder: {subDirName}" );
					ImGui.Separator();

					if ( ImGui.Selectable( "Show in Explorer" ) )
						StartProcess( "explorer.exe", $"/select,\"{subDir}\"" );

					ImGui.EndPopup();
				}

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
			if ( searchBoxText.Length > 0 && !file.Contains( searchBoxText ) )
				continue;

			var fileHandler = FindFileHandler( Path.GetExtension( file ) );
			var icon = fileHandler.Icon;

			DrawIcon( icon, Path.GetFileName( file ) );

			//
			// Context menu
			//
			if ( ImGui.BeginPopupContextItem() )
			{
				ImGui.Text( $"File: {Path.GetFileName( file )}" );
				ImGui.Separator();

				if ( ImGui.Selectable( "Show in Explorer" ) )
					StartProcess( "explorer.exe", $"/select,\"{file}\"" );

				if ( ImGui.Selectable( "Open in Hex Editor" ) )
				{
					var fileData = File.ReadAllBytes( file );
					subTabs.Add( new FilePreviewTab( fileData, new GenericFileHandler( fileData ) ) );
				}

				if ( ImGui.Selectable( "Open in HxD" ) )
					StartProcess( @"C:\Program Files\HxD\HxD.exe", $"\"{file}\"" );

				if ( ImGui.Selectable( "Open in Notepad" ) )
					StartProcess( @"notepad.exe", $"\"{file}\"" );


				ImGui.EndPopup();
			}

			if ( ImGui.IsItemClicked() )
			{
				var fileData = File.ReadAllBytes( file );

				var args = new object[] { fileData };
				selectedFileHandler = Activator.CreateInstance( fileHandler.Type, args ) as BaseFileHandler;

				subTabs.Add( new FilePreviewTab( fileData, selectedFileHandler ) );
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
		// Search box
		//
		{
			ImGui.SetNextItemWidth( -1 );
			ImGui.InputText( "##search", ref searchBoxText, 256 );
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
		ImGui.End();

		subTabs.ForEach( x => x.Draw() );
	}
}
