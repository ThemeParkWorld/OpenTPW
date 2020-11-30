using Engine.ECS.Components;
using Engine.ECS.Observer;
using Engine.Types;
using System;
using System.Collections.Generic;

namespace Engine.ECS.Entities
{
    public interface IEntity : IHasParent
    {
        string Name { get; set; }
        List<string> Tags { get; set; }

        List<IComponent> Components { get; }
        string IconGlyph { get; }
        bool Enabled { get; }
        Guid Id { get; }

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
        /// <param name="deltaTime"></param>
        void Update(float deltaTime);

        /// <summary>
        /// Get a component of type T from the Component's entity list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetComponent<T>();

        /// <summary>
        /// Check whether the entity has a specific Component attached.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasComponent<T>();

        /// <summary>
        /// Add a component to an entity. This will also check for component dependencies.
        /// </summary>
        /// <param name="component">An instance of the desired component to add.</param>
        void AddComponent(IComponent component);
    }
}
