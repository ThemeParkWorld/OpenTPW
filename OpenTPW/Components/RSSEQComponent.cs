using ECSEngine.Components;
using ImGuiNET;
using OpenTPW.RSSEQ;

namespace OpenTPW.Components
{
    public class RSSEQComponent : Component<RSSEQComponent>
    {
        private VM vmInstance; // probably shouldn't create a new VM instance for every ride
        private bool showImGUIWindow;
        private string disassembly;
        private string log;

        public RSSEQComponent(string pathToRSE)
        {
            vmInstance = new VM(pathToRSE);
            disassembly = vmInstance.Disassembly;
        }

        public RSSEQComponent(byte[] rseData)
        {
            vmInstance = new VM(rseData);
            disassembly = vmInstance.Disassembly;
        }

        public override void RenderImGUI()
        {
            if (ImGui.Button("Toggle window"))
            {
                showImGUIWindow = !showImGUIWindow;
            }

            if (showImGUIWindow)
            {
                ImGui.Begin("RSSEQ VM");

                if (ImGui.Button("Step thru"))
                {
                    vmInstance.Step();
                }

                ImGui.LabelText("", "Config");
                // TODO: condense these into a list somehow
                foreach (var property in typeof(VM).GetProperties())
                {
                    var propName = property.Name;
                    var propValue = property.GetValue(vmInstance);
                    if (propValue != null)
                        ImGui.LabelText(propName, propValue.ToString());
                }

                ImGui.LabelText("", "Variables");
                // pipe all variables thru to imgui
                for (var i = 0; i < vmInstance.Variables.Count; i++)
                {
                    ImGui.LabelText(vmInstance.Variables[i].ToString(), i.ToString());
                }

                //ImGui.LabelText("", "Disassembly");

                //ImGui.PushItemWidth(-1);
                //ImGui.InputTextMultiline("Disassembly", ref disassembly, UInt32.MaxValue, new Vector2(-1, 250));
                //ImGui.PopItemWidth();

                ImGui.End();
            }
        }
    }
}
