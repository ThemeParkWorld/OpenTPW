using Engine.Assets;
using Engine.ECS.Components;
using Engine.Gui.Attributes;
using ImGuiNET;
using OpenTPW.RSSEQ;
using System;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace OpenTPW.Components
{
    public class RSSEQComponent : Component<RSSEQComponent>
    {
        private VM vmInstance;
        private bool showImGUIWindow;

        public RSSEQComponent(string pathToRSE)
        {
            vmInstance = new VM(pathToRSE);
        }

        public RSSEQComponent(byte[] rseData)
        {
            vmInstance = new VM(rseData);
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
            ImGui.Begin("RSSEQ VM");

            if (ImGui.Button(FontAwesome5.StepForward))
            {
                vmInstance.Step();
            }

            if (ImGui.TreeNode("Config"))
            {
                ImGui.Columns(2, null, false);
                foreach (var property in typeof(VM).GetProperties())
                {
                    var propName = property.Name;
                    var propValue = property.GetValue(vmInstance);

                    if (propValue == null || propName == null || property.GetCustomAttribute(typeof(HideInImGui)) != null)
                        continue;

                    ImGui.Text(propName);
                    ImGui.NextColumn();
                    ImGui.Text(propValue.ToString());
                    ImGui.NextColumn();
                }
                ImGui.Columns(1);
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Variables"))
            {
                ImGui.Columns(2, null, false);
                // pipe all variables thru to imgui
                for (var i = 0; i < vmInstance.Variables.Count; i++)
                {
                    ImGui.Text(vmInstance.VariableNames[i]);
                    ImGui.NextColumn();
                    var variableValue = vmInstance.Variables[i];
                    if (ImGui.DragInt($"##hidelabel{vmInstance.VariableNames[i]}", ref variableValue))
                        vmInstance.Variables[i] = variableValue;
                    ImGui.NextColumn();
                }
                ImGui.Columns(1);
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Disassembly"))
            {
                ImGui.Columns(8, null, false);
                ImGuiHighlightBackground(new Vector4(1, 0, 1, 1));
                int instructionIndex = 0;
                int jumpOffset = 0;
                foreach (var instruction in vmInstance.Instructions)
                {
                    if (vmInstance.Branches.Any(b => b.compiledOffset == jumpOffset - 1))
                    {
                        ImGui.Text($".jump_{jumpOffset - 1}");
                        ImGui.NextColumn();
                        
                        for (int i = 0; i < 7; ++i)
                        {
                            ImGui.NextColumn();
                        }
                    }
                    
                    ImGui.Text((vmInstance.CurrentPos == instructionIndex) ? FontAwesome5.ChevronRight : "");
                    ImGui.NextColumn();
                    ImGui.Text(instruction.opcode.ToString());

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        var instructionHandler = vmInstance.FindOpcodeHandler(instruction.opcode);
                        ImGui.Text(instruction.opcode.ToString());
                        if (instructionHandler == null)
                        {
                            ImGui.Text("No handler found.");
                        }
                        else
                        {
                            ImGui.Text(instructionHandler.Description);
                            if (instructionHandler.OpcodeIds.Length > 1)
                            {
                                var extensionListStr = "";
                                for (int i = 1; i < instructionHandler.OpcodeIds.Length ; ++i)
                                {
                                    extensionListStr += instructionHandler.OpcodeIds[i].ToString();
                                    if (i != instructionHandler.OpcodeIds.Length - 1)
                                        extensionListStr += ", ";
                                }
                                ImGui.Text($"Extensions: {extensionListStr}");
                            }

                            if (instructionHandler.Args.Length > 0)
                            {
                                var argStr = "";
                                for (int i = 0; i < instructionHandler.Args.Length ; ++i)
                                {
                                    argStr += instructionHandler.Args[i].ToString();
                                    if (i != instructionHandler.Args.Length - 1)
                                        argStr += ", ";
                                }
                                ImGui.Text($"Args: {argStr}");
                            }
                        }
                        ImGui.EndTooltip();
                    }

                    ImGui.NextColumn();
                    for (int i = 0; i < 6; ++i)
                    {
                        if (i <= instruction.operands.Length - 1 && instruction.operands.Length > 0)
                            ImGui.Text(instruction.operands[i].ToString());
                        ImGui.NextColumn();
                    }

                    instructionIndex++;
                    jumpOffset += instruction.GetCount();
                }
                ImGui.Columns(1);
                ImGui.TreePop();
            }

            ImGui.End();
        }

        // Adapted from https://github.com/ocornut/imgui/issues/2668
        private void ImGuiHighlightBackground(Vector4 color, float lineHeight = -1.0f)
        {
            var drawList = ImGui.GetWindowDrawList();
            var style = ImGui.GetStyle();

            if (lineHeight < 0)
            {
                lineHeight = ImGui.GetTextLineHeight();
            }

            lineHeight += style.ItemSpacing.Y;
            float scrollOffsetX = ImGui.GetScrollX();
            float scrollOffsetY = ImGui.GetScrollY();
            float scrolledOutlines = (float)(Math.Floor(scrollOffsetY / lineHeight));
            scrollOffsetY -= lineHeight * scrolledOutlines;

            Vector2 clipRectMin = ImGui.GetWindowPos();
            Vector2 clipRectMax = new Vector2(
                clipRectMin.X + ImGui.GetWindowWidth(),
                clipRectMin.Y + ImGui.GetWindowHeight()
            );

            if (ImGui.GetScrollMaxX() > 0)
            {
                clipRectMax.Y -= style.ScrollbarSize;
            }

            // drawList.PushClipRect(clipRectMin, clipRectMax);

            bool isOdd = ((int)scrolledOutlines % 2 == 0);
            
            float yMin = clipRectMin.Y - scrollOffsetY + ImGui.GetCursorPosY();
            float yMax = clipRectMax.Y - scrollOffsetY + lineHeight;
            float xMin = clipRectMin.X + scrollOffsetX + ImGui.GetWindowContentRegionMin().X;
            float xMax = clipRectMin.X + scrollOffsetX + ImGui.GetWindowContentRegionMax().X;

            for (float y = yMin; y < yMax; y += lineHeight, isOdd = !isOdd)
            {
                if (isOdd)
                {
                    drawList.AddRectFilled(new Vector2(xMin, y - style.ItemSpacing.Y), new Vector2(xMax, y + lineHeight), ImGuiNative.igColorConvertFloat4ToU32(color));
                }
            }

            // drawList.PopClipRect();
        }
    }
}
