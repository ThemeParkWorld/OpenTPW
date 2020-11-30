using System;

namespace Engine.Utils.Attributes
{
    /// <summary>
    /// Prevents an item from being shown in ImGui windows.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public sealed class HideInImGuiAttribute : Attribute { }
}
