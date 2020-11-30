using Engine.Assets;
using Engine.ECS.Entities;
using ImGuiNET;
using Quincy.Managers;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    [ImGuiMenuPath(ImGuiMenus.Menu.Scene)]
    class ScenePropertiesWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Sitemap;
        public override string Title { get; } = "Scene Properties";

        private int currentSceneHierarchyItem;
        private IEntity selectedEntity;

        public override void Draw()
        {
            ImGui.Button(FontAwesome5.Plus);
            ImGui.SameLine();
            ImGui.Button(FontAwesome5.Trash);
            ImGui.SameLine();
            ImGui.Button(FontAwesome5.Clone);
            ImGui.Separator();

            var entityNames = new string[SceneManager.Instance.Entities.Count];
            for (var i = 0; i < SceneManager.Instance.Entities.Count; i++)
            {
                entityNames[i] = $"{SceneManager.Instance.Entities[i].IconGlyph} {SceneManager.Instance.Entities[i].Name ?? SceneManager.Instance.Entities[i].GetType().Name}";
            }

            ImGui.PushItemWidth(-1);
            if (ImGui.ListBox("Hierarchy", ref currentSceneHierarchyItem, entityNames, entityNames.Length))
            {
                selectedEntity = SceneManager.Instance.Entities[currentSceneHierarchyItem];
            }
            ImGui.PopItemWidth();

            ImGui.Separator();

            if (selectedEntity == null)
            {
                ImGui.End();
                return;
            }

            selectedEntity.RenderImGui();
        }
    }
}
