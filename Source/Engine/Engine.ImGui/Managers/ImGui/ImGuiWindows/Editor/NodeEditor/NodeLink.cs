namespace Engine.Gui.Managers.ImGuiWindows.Editor.NodeEditor
{
    public struct NodeLink
    {
        public NodeLink(int inputId, int inputSlot, int outputId, int outputSlot)
        {
            InputId = inputId;
            InputSlot = inputSlot;
            OutputId = outputId;
            OutputSlot = outputSlot;
        }

        public int InputId { get; set; }
        public int InputSlot { get; set; }
        public int OutputId { get; set; }
        public int OutputSlot { get; set; }
    }
}
