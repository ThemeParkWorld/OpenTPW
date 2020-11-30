using System;

namespace Engine.Gui.Managers.ImGuiWindows
{
    internal class ImGuiMenuPathAttribute : Attribute
    {
        public ImGuiMenus.Menu Path { get; }
        public ImGuiMenuPathAttribute(ImGuiMenus.Menu path)
        {
            Path = path;
        }
    }
}