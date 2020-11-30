using Engine.Assets;
using Engine.ECS.Observer;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Editor.NodeEditor
{
    // Based on https://gist.github.com/ocornut/7e9b3ec566a333d725d4
    [ImGuiMenuPath(ImGuiMenus.Menu.Experimental)]
    public class NodeEditorWindow : ImGuiWindow
    {
        public override bool Render { get; set; }

        public override string Title => "Node Editor";

        public override string IconGlyph => FontAwesome5.Edit;

        private List<Node> nodes;
        private List<NodeLink> links;
        private Vector2 scrolling = new Vector2(0f, 0f);
        private bool showGrid = true;
        private const float GRID_SIZE = 32.0f;

        private int nodeSelected = -1;

        private Vector2 nodeIoPosition = new Vector2(0, 0);

        /// <summary>
        /// Converts 0-255 RGBA into an imgui-compatible uint
        /// </summary>
        /// <param name="rgbColor"></param>
        /// <returns></returns>
        private uint GetImGuiColor(Vector4 rgbColor)
        {
            return ImGui.GetColorU32(rgbColor / 255.0f);
        }

        public override void Draw()
        {
            bool openContextMenu = false;
            int nodeHoveredInScene = -1;

            var io = ImGui.GetIO();

            ImGui.BeginGroup();

            float nodeSlotRadius = 4.0f;

            var slotActivationScale = 2.0f;
            var nodeSlotRadiusVec = new Vector2(nodeSlotRadius, nodeSlotRadius) * slotActivationScale;
            var halfNodeSlotRadiusVec = nodeSlotRadiusVec / 2f;

            Vector2 nodeWindowPadding = new Vector2(8f, 8f);

            ImGui.Checkbox("Show grid", ref showGrid);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(1, 1));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, new Vector2(0, 0));
            ImGui.PushStyleColor(ImGuiCol.ChildBg, GetImGuiColor(new Vector4(60, 60, 70, 200)));
            ImGui.BeginChild("ScrollingRegion", new Vector2(0, 0), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);
            ImGui.PopStyleVar();
            ImGui.PushItemWidth(120.0f);

            Vector2 offset = ImGui.GetCursorScreenPos() + scrolling;
            var drawList = ImGui.GetWindowDrawList();

            if (showGrid)
            {
                var gridColor = GetImGuiColor(new Vector4(200, 200, 200, 40));
                var winPos = ImGui.GetCursorScreenPos();
                var canvasSize = ImGui.GetWindowSize();

                for (float x = scrolling.X % GRID_SIZE; x < canvasSize.X; x += GRID_SIZE)
                    drawList.AddLine(new Vector2(x, 0.0f) + winPos, new Vector2(x, canvasSize.Y) + winPos, gridColor);
                for (float y = scrolling.Y % GRID_SIZE; y < canvasSize.Y; y += GRID_SIZE)
                    drawList.AddLine(new Vector2(0.0f, y) + winPos, new Vector2(canvasSize.X, y) + winPos, gridColor);
            }

            drawList.ChannelsSplit(2);
            drawList.ChannelsSetCurrent(0);

            foreach (var link in links)
            {
                var nodeInp = nodes[link.InputId];
                var nodeOut = nodes[link.OutputId];

                var p1 = offset + nodeInp.GetOutputSlotPos(link.InputSlot);
                var p2 = offset + nodeOut.GetInputSlotPos(link.OutputSlot);

                drawList.AddBezierCurve(p1, p1 + new Vector2(50, 0), p2 + new Vector2(-50, 0), p2, GetImGuiColor(new Vector4(200, 200, 100, 255)), 3.0f);
            }

            bool nodeIoActive = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                ImGui.PushID(node.Id);
                var nodeRectMin = offset + node.Position;
                drawList.ChannelsSetCurrent(1);
                bool oldAnyActive = ImGui.IsAnyItemActive();
                ImGui.SetCursorScreenPos(nodeRectMin + nodeWindowPadding);
                ImGui.BeginGroup();
                ImGui.Text($"{node.Name}");

                ImGuiUtils.RenderImGuiMembers(node);

                ImGui.EndGroup();

                bool nodeWidgetsActive = (!oldAnyActive && ImGui.IsAnyItemActive());
                node.Size = ImGui.GetItemRectSize() + nodeWindowPadding + nodeWindowPadding;
                var nodeRectMax = nodeRectMin + node.Size;

                drawList.ChannelsSetCurrent(0);
                ImGui.SetCursorScreenPos(nodeRectMin);
                ImGui.InvisibleButton("node", node.Size);
                if (ImGui.IsItemHovered())
                {
                    nodeHoveredInScene = node.Id;
                    openContextMenu |= ImGui.IsMouseClicked(ImGuiMouseButton.Right);
                }
                bool nodeMovingActive = ImGui.IsItemActive();

                if (nodeWidgetsActive || nodeMovingActive)
                    nodeSelected = node.Id;

                if (nodeMovingActive && ImGui.IsMouseDragging(ImGuiMouseButton.Left))
                {
                    node.Position += io.MouseDelta;
                }

                var nodeBgColor = (nodeHoveredInScene == node.Id || nodeSelected == node.Id) ? GetImGuiColor(new Vector4(75, 75, 75, 255)) : GetImGuiColor(new Vector4(60, 60, 60, 255));

                drawList.AddRectFilled(nodeRectMin, nodeRectMax, nodeBgColor, 4.0f);
                drawList.AddRectFilled(nodeRectMin, nodeRectMax, GetImGuiColor(new Vector4(100, 100, 100, 255)), 4.0f);
                
                for (int slotId = 0; slotId < node.InputCount; ++slotId)
                {
                    var circlePos = offset + node.GetInputSlotPos(slotId);
                    ImGui.SetCursorScreenPos(circlePos - halfNodeSlotRadiusVec);
                    ImGui.InvisibleButton($"input{slotId}", nodeSlotRadiusVec);
                    if (ImGui.IsItemActive())
                    {
                        nodeIoActive = true;
                        nodeIoPosition = node.GetInputSlotPos(slotId);
                    }

                    bool inputHovered = ImGui.IsItemHovered();
                    var color = inputHovered ? GetImGuiColor(new Vector4(150, 255, 150, 150)) : GetImGuiColor(new Vector4(150, 150, 150, 150));
                    drawList.AddCircleFilled(circlePos, nodeSlotRadius, color);
                }
                for (int slotId = 0; slotId < node.OutputCount; ++slotId)
                {
                    var circlePos = offset + node.GetOutputSlotPos(slotId);
                    ImGui.SetCursorScreenPos(circlePos - halfNodeSlotRadiusVec);
                    ImGui.InvisibleButton($"output{slotId}", nodeSlotRadiusVec);
                    if (ImGui.IsItemActive())
                    {
                        nodeIoActive = true;
                        nodeIoPosition = node.GetOutputSlotPos(slotId);
                    }

                    bool outputHovered = ImGui.IsItemHovered();
                    var color = outputHovered ? GetImGuiColor(new Vector4(150, 255, 150, 150)) : GetImGuiColor(new Vector4(150, 150, 150, 150));
                    drawList.AddCircleFilled(circlePos, nodeSlotRadius, color);
                }

                ImGui.PopID();
            }

            if (nodeIoActive && ImGui.IsMouseDragging(ImGuiMouseButton.Left))
            {
                var mousePos = ImGui.GetMousePos();
                drawList.AddBezierCurve(offset + nodeIoPosition, offset + nodeIoPosition + new Vector2(50, 0), mousePos + new Vector2(-50, 0), mousePos, GetImGuiColor(new Vector4(200, 200, 100, 255)), 3.0f);
            }

            drawList.ChannelsMerge();

            if (ImGui.IsMouseReleased(ImGuiMouseButton.Right))
            {
                if (ImGui.IsWindowHovered(ImGuiHoveredFlags.AllowWhenBlockedByPopup) || !ImGui.IsAnyItemHovered())
                {
                    nodeSelected = nodeHoveredInScene = -1;
                    nodeIoActive = false;
                    openContextMenu = true;
                }
            }

            if (openContextMenu)
            {
                ImGui.OpenPopup("ContextMenu");
                if (nodeHoveredInScene != -1)
                    nodeSelected = nodeHoveredInScene;
            }

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(8, 8));
            if (ImGui.BeginPopup("ContextMenu"))
            {
                Node node = nodeSelected != -1 ? nodes[nodeSelected] : null;

                var scenePos = ImGui.GetMousePosOnOpeningCurrentPopup() - offset;
                if (node != null)
                {
                    ImGui.Text($"Node {node.Name}");
                    ImGui.Separator();
                    if (ImGui.MenuItem("Rename")) { }
                    if (ImGui.MenuItem("Delete")) { }
                    if (ImGui.MenuItem("Copy")) { }
                }
                else
                {
                    if (ImGui.MenuItem("Add")) 
                    {
                        nodes.Add(new PbrNode(nodes.Count, "Principled BRDF", scenePos, new Vector2(0, 0), 0.5f, new Vector4(100, 100, 200, 255) / 255.0f, 2, 2)); 
                    }
                    if (ImGui.MenuItem("Paste")) { }
                }

                ImGui.EndPopup();
            }
            ImGui.PopStyleVar();

            if (ImGui.IsWindowHovered() && !ImGui.IsAnyItemActive() && ImGui.IsMouseDragging(ImGuiMouseButton.Middle, 0.0f))
                scrolling += io.MouseDelta;  

            ImGui.PopItemWidth();
            ImGui.EndChild();
            ImGui.PopStyleColor();
            ImGui.EndGroup();
        }

        public override void OnNotify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            if (eventType == NotifyType.ContextReady)
            {
                nodes = new List<Node>();
                links = new List<NodeLink>();

                nodes.Add(new Node(0, "MainTex", new Vector2(40, 50), new Vector2(500, 250), 0.5f, new Vector4(255, 100, 100, 255) / 255.0f, 1, 1));
                nodes.Add(new Node(1, "BumpMap", new Vector2(40, 150), new Vector2(500, 250), 0.42f, new Vector4(255, 100, 100, 255) / 255.0f, 1, 1));
                nodes.Add(new Node(2, "Combine", new Vector2(270, 80), new Vector2(500, 250), 1.0f, new Vector4(255, 100, 100, 255) / 255.0f, 2, 1));

                links.Add(new NodeLink(0, 0, 2, 0));
                links.Add(new NodeLink(1, 0, 2, 1));
            }
            base.OnNotify(eventType, notifyArgs);
        }
    }
}
