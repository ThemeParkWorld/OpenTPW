using Engine.ECS.Entities;
using Engine.ECS.Observer;
using Engine.Types;
using Engine.Utils.DebugUtils;
using ImGuiNET;

namespace Engine.ECS.Components
{
    /// <summary>
    /// The base class for any component running in the engine.
    /// </summary>
    public class Component<T> : IComponent
    {
        /// <summary>
        /// The component's parent; usually an entity.
        /// </summary>
        public virtual IHasParent Parent { get; set; }

        /// <summary>
        /// Display draw all available properties within ImGUI.
        /// </summary>
        public virtual void RenderImGui()
        {
            ImGuiUtils.RenderImGuiMembers(this);
        }

        private ImGuiInputTextFlags GetFlags(dynamic reference) => reference.CanGet ? ImGuiInputTextFlags.None : ImGuiInputTextFlags.ReadOnly;

        /// <summary>
        /// Called whenever the engine renders a single frame.
        /// </summary>
        public virtual void Render() { }

        /// <summary>
        /// Called when an notification is broadcast.
        /// </summary>
        /// <param name="notifyType">The type of the notification broadcast.</param>
        /// <param name="notifyArgs">Any relevant information about the notification.</param>
        public virtual void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs) { }

        /// <summary>
        /// Called whenever the engine wishes to update all systems/entities/components.
        /// </summary>
        public virtual void Update(float deltaTime) { }

        /// <summary>
        /// Get a component of type T1 from the Component's entity list.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        protected virtual T1 GetComponent<T1>()
        {
            return ((IEntity)Parent).GetComponent<T1>();
        }
    }
}
