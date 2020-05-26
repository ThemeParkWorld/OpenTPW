using Engine.ECS.Components;
using Engine.Gui.Attributes;
using ImGuiNET;
using OpenTPW.Files;
using OpenTPW.Files.FileFormats;
using OpenTPW.RSSEQ;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenTPW.Components
{
    public class SAMComponent : Component<SAMComponent>
    {
        private bool showImGUIWindow;
        private List<SAMPair> samContents;

        public SAMComponent(byte[] samData)
        {
            samContents = new SAMReader().LoadAsset(samData).Data.Cast<SAMPair>().ToList();
        }

        public override void RenderImGui()
        {
            if (ImGui.Button("Toggle window"))
            {
                showImGUIWindow = !showImGUIWindow;
            }

            if (showImGUIWindow)
            {
                DrawImGuiWindow();
            }
        }

        private void DrawImGuiWindow()
        {
            ImGui.Begin("Settings & Modifiers");

            ImGui.Columns(2, null, false);
            foreach (var kvp in samContents)
            {
                ImGui.Text(kvp.Key);
                ImGui.NextColumn();
                ImGui.Text(kvp.Value);
                ImGui.NextColumn();
            }
            ImGui.Columns(1);
            ImGui.TreePop();

            ImGui.End();
        }
    }
}
