using ECSEngine.Components;
using ImGuiNET;
using OpenTPW.RSSEQ;
using System;
using System.Numerics;

namespace OpenTPW.Components
{
    public class RSSEQComponent : Component<RSSEQComponent>
    {
        private VM vm; // probably shouldn't create a new VM instance for every ride
        private bool showImGUIWindow;
        private string disassembly;

        public RSSEQComponent(string pathToRSE)
        {
            vm = new VM(pathToRSE);
            disassembly = vm.disassembly;
        }

        public RSSEQComponent(byte[] rseData)
        {
            vm = new VM(rseData);
            disassembly = vm.disassembly;
        }

        public override void RenderImGUI()
        {
            if (ImGui.Button("Toggle window"))
            {
                showImGUIWindow = !showImGUIWindow;
            }

            if (showImGUIWindow)
            {
                ImGui.Begin("Test");
                // TODO: condense these into a list somehow
                ImGui.InputInt("Stack Size", ref vm.stackSize);
                ImGui.InputInt("Limbo Size", ref vm.limboSize);
                ImGui.InputInt("Bounce Size", ref vm.bounceSize);
                ImGui.InputInt("Walk Size", ref vm.walkSize);
                ImGui.InputInt("Time Slice", ref vm.timeSlice);

                ImGui.LabelText("", "RSSEQ Disassembly");

                ImGui.PushItemWidth(-1);
                ImGui.InputTextMultiline("Disassembly", ref disassembly, UInt32.MaxValue, new Vector2(-1, 250));
                ImGui.PopItemWidth();

                ImGui.End();
            }
        }
    }
}
