using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL.Extensions.ImGui;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OpenTPW;
internal partial class Editor
{
    private ImGuiController ImGuiController;
    private Texture defaultFontTexture;

    private List<BaseTab> tabs = new();

    private bool shouldRender;

    public static ImFontPtr MonospaceFont { get; private set; }
    public static ImFontPtr SansSerifFont { get; private set; }

    public Editor( ImGuiController imGuiController )
    {
        ImGuiController = imGuiController;

        InitIO();
        SetTheme();

        tabs.AddRange( new BaseTab[] {
            new TexturesTab(),
            new ConsoleTab(),
            new InputTab(),
            new RidesTab(),
            new FileBrowserTab(),
            new DemoWindow(),
            new CursorTab()
        } );
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
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.Backspace;
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

        io.Fonts.SetTexID( (IntPtr)defaultFontTexture.Id );
        io.Fonts.ClearTexData();

        SetKeyMappings( io );
    }

    private void DrawMenuBar()
    {
        ImGui.BeginMainMenuBar();

        ImGui.Text( "OpenTPW |" );

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

    public void Update()
    {
        ImGuiController.Update( Time.Delta );

        if ( Input.Pressed( InputButton.ConsoleToggle ) )
            shouldRender = !shouldRender;

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

