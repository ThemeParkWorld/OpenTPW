using Engine.Assets;
using Engine.ECS.Components;
using Engine.Gui.Attributes;
using ImGuiNET;
using OpenTPW.Files.FileFormats;
using OpenTPW.Files.FileFormats.BFWD;
using OpenTPW.RSSEQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenTPW.Components
{
    class RideComponent : Component<RideComponent>
    {
        private List<SAMPair> samContents;
        private VM vmInstance;
        private bool showImGUIWindow;

        public RideComponent(string rideArchivePath)
        {
            var rideName = Path.GetFileNameWithoutExtension(rideArchivePath);
            var rideArchive = new BFWDArchive(rideArchivePath);

            var rseFile = rideArchive.files.First(file => file.name.Equals($"{rideName}.RSE\0", StringComparison.OrdinalIgnoreCase));
            var samFile = rideArchive.files.First(file => file.name.Equals($"{rideName}.SAM\0", StringComparison.OrdinalIgnoreCase));

            vmInstance = new VM(rseFile.data);
            samContents = new SAMReader().LoadAsset(samFile.data).Data.Cast<SAMPair>().ToList();
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

            ImGui.ShowStyleEditor();
        }

        private void DrawProperties()
        {
            ImGui.Columns(2, null, false);
            foreach (var kvp in samContents)
            {
                ImGui.Text(kvp.Key);
                ImGui.NextColumn();
                ImGui.Text(kvp.Value);
                ImGui.NextColumn();
            }
            ImGui.Columns(1);
        }

        private void DrawConfig()
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
        }

        private void DrawVariables()
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
        }

        private void DrawDisassembly()
        {
            if (ImGui.Button(FontAwesome5.Play))
            { }
            ImGui.SameLine();
            if (ImGui.Button(FontAwesome5.Redo))
            { }
            ImGui.SameLine();
            if (ImGui.Button(FontAwesome5.StepForward))
            {
                vmInstance.Step();
            }
            ImGui.SameLine();
            ImGui.Text($"Offset {vmInstance.CurrentPos}");

            ImGui.Separator();

            ImGui.Columns(8, null, false);
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
                            for (int i = 1; i < instructionHandler.OpcodeIds.Length; ++i)
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
                            for (int i = 0; i < instructionHandler.Args.Length; ++i)
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
        }

        private void DrawImGuiWindow()
        {
            ImGui.Begin("Ride Info");

            ImGui.Columns(2, "mainColumns", false);

            ImGui.BeginChild("column1");

            if (ImGui.TreeNode("Properties (SAM)"))
            {
                ImGui.BeginChild("properties");
                DrawProperties();
                ImGui.EndChild();
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Config"))
            {
                ImGui.BeginChild("config");
                DrawConfig();
                ImGui.EndChild();
                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Variables"))
            {
                ImGui.BeginChild("variables");
                DrawVariables();
                ImGui.EndChild();
                ImGui.TreePop();
            }

            ImGui.EndChild();

            ImGui.NextColumn();
            ImGui.SetColumnWidth(0, 400);

            //if (ImGui.TreeNode("Disassembly"))
            //{
                ImGui.BeginChild("disassembly");
                DrawDisassembly();
                ImGui.EndChild();
            //}

            ImGui.Columns(1);

            ImGui.End();
        }
    }
}
