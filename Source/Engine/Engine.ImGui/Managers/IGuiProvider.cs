using Engine.ECS.Observer;
using Engine.Gui.Managers.ImGuiWindows;
using Engine.Gui.Managers.ImGuiWindows.Theming;
using ImGuiNET;
using System.Collections.Generic;

namespace Engine.Gui.Managers
{
    public interface IGuiProvider
    {
        List<ImGuiMenu> Menus { get; }
        ImFontPtr MonospacedFont { get; }
        List<ImGuiWindow> Overlays { get; }
        ImGuiTheme Theme { get; set; }

        void OnNotify(NotifyType eventType, INotifyArgs notifyArgs);
        void Run();
    }
}