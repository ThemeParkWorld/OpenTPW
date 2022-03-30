using ImGuiNET;
using System.Reflection;
using System.Runtime.InteropServices;
using Veldrid;

namespace OpenTPW;
internal partial class Editor
{
	public static Editor Instance { get; private set; }

	public ImGuiRenderer ImGuiRenderer { get; private set; }
	private Texture defaultFontTexture;

	private List<BaseTab> tabs = new();

	private bool shouldRender;

	public static ImFontPtr MonospaceFont { get; private set; }
	public static ImFontPtr SansSerifFont { get; private set; }

	public Editor( ImGuiRenderer imGuiController )
	{
		Instance ??= this;

		ImGuiRenderer = imGuiController;

		InitIO();
		SetTheme();

		tabs.AddRange( new BaseTab[] {
			new TexturesTab(),
			new ConsoleTab(),
			new InputTab(),
			new RidesTab(),
			new FileBrowserTab(),
			new DemoWindow(),
			new CursorTab(),
			new SceneTab()
		} );

		tabs.ForEach( x => x.ImGuiRenderer = ImGuiRenderer );
	}

	private static void SetKeyMappings( ImGuiIOPtr io )
	{
		io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab;
		io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left;
		io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right;
		io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up;
		io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down;
		io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp;
		io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown;
		io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home;
		io.KeyMap[(int)ImGuiKey.End] = (int)Key.End;
		io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete;
		io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.BackSpace;
		io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter;
		io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape;
		io.KeyMap[(int)ImGuiKey.A] = (int)Key.A;
		io.KeyMap[(int)ImGuiKey.C] = (int)Key.C;
		io.KeyMap[(int)ImGuiKey.V] = (int)Key.V;
		io.KeyMap[(int)ImGuiKey.X] = (int)Key.X;
		io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y;
		io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z;
	}

	private void InitIO()
	{
		var io = ImGui.GetIO();

		io.ConfigFlags |= ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.IsSRGB;

		io.Fonts.Clear();

		SansSerifFont = io.Fonts.AddFontFromFileTTF( "content/fonts/Roboto-Regular.ttf", 14f );
		MonospaceFont = io.Fonts.AddFontDefault();

		io.Fonts.GetTexDataAsRGBA32( out IntPtr pixels, out var width, out var height, out var bpp );

		int size = width * height * bpp;
		byte[] data = new byte[size];
		Marshal.Copy( pixels, data, 0, size );
		defaultFontTexture = TextureBuilder.FromBytes( data, (uint)width, (uint)height ).Build();

		var texPtr = ImGuiRenderer.GetOrCreateImGuiBinding( Device.ResourceFactory, defaultFontTexture.VeldridTextureView );
		io.Fonts.SetTexID( texPtr );
		io.Fonts.ClearTexData();

		SetKeyMappings( io );
	}

	private void DrawPerfOverlay()
	{
		var io = ImGui.GetIO();
		var window_flags = ImGuiWindowFlags.NoDecoration |
			ImGuiWindowFlags.AlwaysAutoResize |
			ImGuiWindowFlags.NoSavedSettings |
			ImGuiWindowFlags.NoFocusOnAppearing |
			ImGuiWindowFlags.NoNav |
			ImGuiWindowFlags.NoInputs |
			ImGuiWindowFlags.NoMove;

		const float padding = 8.0f;

		var viewport = ImGui.GetMainViewport();
		var workPos = viewport.WorkPos; // Use work area to avoid menu-bar/task-bar, if any!

		System.Numerics.Vector2 windowPos, windowPivot;

		windowPos.X = workPos.X + padding;
		windowPos.Y = workPos.Y + padding;
		windowPivot.X = 0.0f;
		windowPivot.Y = 0.0f;

		ImGui.SetNextWindowPos( windowPos, ImGuiCond.Always, windowPivot );

		ImGui.SetNextWindowBgAlpha( 0.5f );

		if ( ImGui.Begin( $"##overlay", window_flags ) )
		{
			string total = GC.GetTotalMemory( false ).ToSize( MathExtensions.SizeUnits.MB );

			ImGui.Text( $"{io.Framerate.CeilToInt()}fps" );
			ImGui.Separator();
			ImGui.Text( $"{total} total" );
		}

		ImGui.End();
	}

	private void DrawMenuBar()
	{
		ImGui.BeginMainMenuBar();

		ImGui.Dummy( new( 4, 0 ) );
		ImGui.Text( "OpenTPW" );
		ImGui.Dummy( new( 4, 0 ) );

		ImGui.Separator();
		ImGui.Dummy( new( 4, 0 ) );

		foreach ( var tab in tabs )
		{
			var editorMenuAttribute = tab.GetType().GetCustomAttribute<EditorMenuAttribute>();
			if ( editorMenuAttribute == null )
				continue;

			var splitPath = editorMenuAttribute.Path.Split( '/' );

			if ( ImGui.BeginMenu( splitPath[0] ) )
			{
				for ( int i = 1; i < splitPath.Length; i++ )
				{
					string? item = splitPath[i];
					bool active = ImGui.MenuItem( item );

					if ( i == splitPath.Length - 1 && active )
						tab.visible = !tab.visible;
				}

				ImGui.EndMenu();
			}
		}

		ImGui.EndMainMenuBar();
	}

	public void UpdateFrom( InputSnapshot inputSnapshot )
	{
		ImGuiRenderer.Update( Time.Delta, inputSnapshot );

		if ( Input.Pressed( InputButton.ConsoleToggle ) )
			shouldRender = !shouldRender;

		DrawPerfOverlay();

		if ( !shouldRender )
			return;

		DrawMenuBar();

		ImGui.DockSpaceOverViewport( ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode );

		tabs.ForEach( tab =>
		{
			if ( tab.visible )
				tab.Draw();
		} );
	}
}

