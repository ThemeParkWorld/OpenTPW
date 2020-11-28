using ImGuiNET;
using OpenGL;
using OpenGL.CoreUI;
using Quincy.MathUtils;
using System;
using System.Numerics;

namespace Quincy
{
    public class QuincyImGui
    {
        private const float PT_TO_PX = 1.3281472327365f;

        private Texture defaultFontTexture;
        private Shader imguiShader;
        private Vector2 windowSize;

        private readonly ImGuiIOPtr io;

        private uint vbo, vao, ebo;

        public ImFontPtr MonospacedFont { get; private set; }

        public QuincyImGui()
        {
            windowSize = new Vector2(Constants.windowWidth, Constants.windowHeight);

            var imGuiContext = ImGui.CreateContext();

            if (ImGui.GetCurrentContext() == IntPtr.Zero)
            {
                ImGui.SetCurrentContext(imGuiContext);
            }

            io = ImGui.GetIO();

            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

            ImGui.StyleColorsDark();

            ImGui.StyleColorsDark();

            InitFonts();
            InitKeymap();
            InitGl();
        }

        private void InitGl()
        {
            imguiShader = new Shader("Content/Shaders/ImGUI/imgui.frag", "Content/Shaders/ImGUI/imgui.vert");

            vao = Gl.GenVertexArray();
            vbo = Gl.GenBuffer();
            ebo = Gl.GenBuffer();
        }

        private void InitFonts()
        {
            var fontSizePixels = PT_TO_PX * 12.5f;

            // Standard fonts
            unsafe
            {
                var stdConfig = ImGuiNative.ImFontConfig_ImFontConfig();

                io.Fonts.AddFontFromFileTTF("Content/Fonts/OpenSans/OpenSans-SemiBold.ttf", fontSizePixels, stdConfig);

                ImGuiNative.ImFontConfig_destroy(stdConfig);
            }

            // Monospaced fonts
            unsafe
            {
                var stdConfig = ImGuiNative.ImFontConfig_ImFontConfig();

                MonospacedFont = io.Fonts.AddFontFromFileTTF("Content/Fonts/IBMPlexMono/IBMPlexMono-SemiBold.ttf", fontSizePixels, stdConfig).NativePtr;

                ImGuiNative.ImFontConfig_destroy(stdConfig);
            }

            io.Fonts.Build();

            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out var width, out var height, out var bpp);
            defaultFontTexture = Texture.LoadFromPtr(pixels, width, height, bpp, "texture_diffuse");
            io.Fonts.SetTexID((IntPtr)defaultFontTexture.Id);
            io.Fonts.ClearTexData();
        }

        private void InitKeymap()
        {
            io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyCode.Tab;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyCode.Return;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyCode.Back;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyCode.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyCode.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyCode.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyCode.Down;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyCode.Delete;
            io.KeyMap[(int)ImGuiKey.Space] = (int)KeyCode.Space;
            io.KeyMap[(int)ImGuiKey.Home] = (int)KeyCode.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)KeyCode.End;

            // Below are for copy, cut, paste, undo, select all, etc.
            io.KeyMap[(int)ImGuiKey.C] = (int)KeyCode.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)KeyCode.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)KeyCode.X;
            io.KeyMap[(int)ImGuiKey.Z] = (int)KeyCode.Z;
            io.KeyMap[(int)ImGuiKey.A] = (int)KeyCode.A;
        }

        public void Render()
        {
            ImGui.NewFrame();
            ImGui.ShowDemoWindow();
            ImGui.Render();
            RenderImGui(ImGui.GetDrawData());
        }

        private void RenderImGui(ImDrawDataPtr drawData)
        {
            // TODO: use abstract graphics impl
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.Disable(EnableCap.CullFace);
            Gl.Disable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.ScissorTest);

            io.DisplaySize = new Vector2(Constants.windowWidth, Constants.windowHeight);
            var projectionMatrix = Matrix4x4f.Ortho2D(0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f);

            imguiShader.Use();
            imguiShader.SetInt("albedoTexture", 0);
            imguiShader.SetMatrix("projection", projectionMatrix);

            Gl.BindVertexArray(vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            Gl.EnableVertexAttribArray(0); // Position
            Gl.EnableVertexAttribArray(1); // UV Coord
            Gl.EnableVertexAttribArray(2); // Colors
            Gl.VertexAttribPointer(0, 2, VertexAttribType.Float, false, 20, (IntPtr)0);
            Gl.VertexAttribPointer(1, 2, VertexAttribType.Float, false, 20, (IntPtr)(2 * sizeof(float)));
            Gl.VertexAttribPointer(2, 4, VertexAttribType.UnsignedByte, true, 20, (IntPtr)(4 * sizeof(float)));

            var clipOffset = drawData.DisplayPos;
            var clipScale = drawData.FramebufferScale;

            for (var commandListIndex = 0; commandListIndex < drawData.CmdListsCount; commandListIndex++)
            {
                int indexOffset = 0;
                var commandList = drawData.CmdListsRange[commandListIndex];

                Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(commandList.VtxBuffer.Size * 20), commandList.VtxBuffer.Data, BufferUsage.DynamicDraw);
                Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)(commandList.IdxBuffer.Size * 2), commandList.IdxBuffer.Data, BufferUsage.DynamicDraw);

                for (var commandIndex = 0; commandIndex < commandList.CmdBuffer.Size; commandIndex++)
                {
                    var currentCommand = commandList.CmdBuffer[commandIndex];

                    var clipBounds = new Vector4f(
                            (currentCommand.ClipRect.X - clipOffset.X) * clipScale.X,
                            (currentCommand.ClipRect.Y - clipOffset.Y) * clipScale.Y,
                            (currentCommand.ClipRect.Z - clipOffset.X) * clipScale.X,
                            (currentCommand.ClipRect.W - clipOffset.Y) * clipScale.Y
                        );
                    Gl.Scissor((int)clipBounds.x, (int)(windowSize.Y - clipBounds.w), (int)(clipBounds.z - clipBounds.x), (int)(clipBounds.w - clipBounds.y));

                    Gl.ActiveTexture(TextureUnit.Texture0);
                    Gl.BindTexture(TextureTarget.Texture2d, defaultFontTexture.Id);

                    Gl.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)currentCommand.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(indexOffset * 2), 0);

                    indexOffset += (int)currentCommand.ElemCount;
                }
            }

            // TODO: Move these elsewhere to prevent later confusion
            Gl.Disable(EnableCap.ScissorTest);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
        }
    }
}
