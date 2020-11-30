using Engine.ECS.Entities;
using Engine.ECS.Observer;
using Engine.Types;
using System;
using System.Collections.Generic;

namespace Engine.ECS.Managers
{
    public class Manager<T> : IManager
    {
        /// <summary>
        /// The manager's parent; usually an instance of Game.
        /// </summary>
        public virtual IHasParent Parent { get; set; }

        public void RenderImGui() { }

        /// <summary>
        /// A list of entities that the manager contains.
        /// </summary>
        public List<IEntity> Entities { get; } = new List<IEntity>();

        // All systems should be singletons.
        private static T privateInstance;

        /// <summary>
        /// Get the single instance of the desired manager.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (privateInstance == null)
                    CreateInstance();
                return privateInstance;
            }
        }

        public static void CreateInstance()
        {
            if (privateInstance == null)
                privateInstance = Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Called when an notification is broadcast.
        /// </summary>
        /// <param name="notifyType">The type of the notification broadcast.</param>
        /// <param name="notifyArgs">Any relevant information about the notification.</param>
        public virtual void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            foreach (var entity in Entities)
            {
                entity.OnNotify(notifyType, notifyArgs);
            }
        }

        /// <summary>
        /// Called whenever the manager should run its typical process. (Usually called in game loop).
        /// </summary>
        public virtual void Run() { }

        public void AddEntity(IEntity entity)
        {
            entity.Parent = this;
            Entities.Add(entity);
        }
    }
}
