using Engine.ECS.Managers;
using Engine.ECS.Observer;
using Engine.Types;
using ImGuiNET;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows
{
    public abstract class ImGuiWindow : IManager
    {
        public abstract bool Render { get; set; }
        public abstract string Title { get; }
        public abstract string IconGlyph { get; }
        public virtual ImGuiWindowFlags Flags { get; }
        public IHasParent Parent { get => ImGuiManager.Instance; set { _ = value; } }

        public abstract void Draw();

        public ImGuiWindow()
        {
            Broadcast.AddManager(this);
        }

        protected void DrawShadowLabel(string str, Vector2? position = null)
        {
            var shadowOffset = new Vector2(1, 1);
            var initialPosition = position ?? ImGui.GetCursorPos();

            ImGui.SetCursorPos(initialPosition + shadowOffset);
            ImGui.TextColored(new Vector4(0, 0, 0, .4f), str);

            ImGui.SetCursorPos(initialPosition);
            ImGui.Text(str);
        }

        public virtual void OnNotify(NotifyType eventType, INotifyArgs notifyArgs) { }

        public void Run() { }

        public void RenderImGui() { }
    }
}
