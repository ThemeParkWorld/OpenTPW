using Engine.ECS.Observer;
using Engine.Types;

namespace Engine.ECS.Components
{
    /// <summary>
    /// The base interface for any component running in the engine.
    /// </summary>
    public interface IComponent : IHasParent
    {
        /// <summary>
        /// Called when an notification is broadcast.
        /// </summary>
        /// <param name="notifyType">The type of the notification broadcast.</param>
        /// <param name="notifyArgs">Any relevant information about the notification.</param>
        void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs);

        /// <summary>
        /// Called whenever the engine renders a single frame.
        /// </summary>
        void Render();

        /// <summary>
        /// Called whenever the engine wishes to update all systems/entities/components.
        /// </summary>
        void Update(float deltaTime);
    }
}
