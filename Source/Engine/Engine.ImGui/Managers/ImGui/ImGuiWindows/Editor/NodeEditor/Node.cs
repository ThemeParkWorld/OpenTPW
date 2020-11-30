using Engine.Utils.Attributes;
using Engine.Utils.MathUtils;
using System;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Editor.NodeEditor
{
    public class Node
    {
        public Node(int id, string name, Vector2 position, Vector2 size, float value, Vector4 color, int inputCount, int outputCount)
        {
            Id = id;
            Name = name;
            Position = position;
            Size = size;
            Value = value;
            Color = color;
            InputCount = inputCount;
            OutputCount = outputCount;
        }

        public Vector2 GetInputSlotPos(int slotNumber)
        {
            return new Vector2(Position.X, Position.Y + Size.Y * ((float)slotNumber + 1) / ((float)InputCount + 1));
        }

        public Vector2 GetOutputSlotPos(int slotNumber)
        {
            return new Vector2(Position.X + Size.X, Position.Y + Size.Y * ((float)slotNumber + 1) / ((float)OutputCount + 1));
        }

        [HideInImGui] public int Id { get; set; }
        [HideInImGui] public string Name { get; set; }
        [HideInImGui] public Vector2 Position { get; set; }
        [HideInImGui] public Vector2 Size { get; set; }
        [Range(0, 1)] public float Value { get; set; }
        [Range(0, 1)] public Vector4 Color { get; set; }
        [HideInImGui] public int InputCount { get; set; }
        [HideInImGui] public int OutputCount { get; set; }
    }

    public class PbrNode : Node
    {
        public PbrNode(int id, string name, Vector2 position, Vector2 size, float value, Vector4 color, int inputCount, int outputCount) : base(id, name, position, size, value, color, inputCount, outputCount)
        { }

        public Vector4 Texture_Diffuse { get; set; }
        public Vector4 Texture_Emissive { get; set; }
        public Vector4 Texture_RMA { get; set; }
        public Vector4 Texture_Normal { get; set; }
        public Vector4 Texture_Specular { get; set; }
    }
}
