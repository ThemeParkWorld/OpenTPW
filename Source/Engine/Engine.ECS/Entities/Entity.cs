using Engine.Assets;
using Engine.ECS.Components;
using Engine.ECS.Observer;
using Engine.Types;
using Engine.Utils.Attributes;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Engine.ECS.Entities
{
    public class Entity<T> : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();

        public bool Enabled { get; private set; } = true;

        private string name;
        public virtual string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return GetType().Name;

                return name;
            }
            set => name = value;
        }

        public virtual string IconGlyph { get; } = FontAwesome5.Box;

        public virtual List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// The entity's parent; usually a manager.
        /// </summary>
        public virtual IHasParent Parent { get; set; }

        /// <summary>
        /// A list of components that the entity contains.
        /// </summary>
        public List<IComponent> Components { get; private set; }

        /// <summary>
        /// Construct an <see cref="Entity{T}"/>.
        /// </summary>
        public Entity()
        {
            Components = new List<IComponent>();
        }

        /// <summary>
        /// Display the available components & their properties
        /// within ImGUI.
        /// </summary>
        public virtual void RenderImGui()
        {
            bool enabled = Enabled;
            ImGui.Checkbox("##hidelabel#enabled", ref enabled);
            Enabled = enabled;

            ImGui.SameLine();

            var nameVal = Name;
            ImGui.InputText("##hidelabel#name", ref nameVal, 256);
            Name = nameVal;

            // Entity info
            ImGui.Text($"{IconGlyph} {GetType().Name}");
            ImGui.Text($"({Id})");

            ImGui.Separator();

            // Components
            foreach (var component in Components)
            {
                if (ImGui.TreeNode(component.GetType().Name))
                {
                    component.RenderImGui();
                    ImGui.TreePop();
                }
            }
        }

        /// <summary>
        /// Check for any components with mismatched or missing dependencies.
        /// </summary>
        private void CheckComponentDependencies()
        {
            /* Some components have "require" attributes to ensure that a specific set of components are met (components
             * that depend on other components, e.g. MeshComponent and ShaderComponent).  We should check these for
             * every entity is created in order to ensure that there are no missing components! */

            foreach (var component in Components)
            {
                foreach (var attribute in component.GetType().GetCustomAttributes())
                {
                    if (attribute is RequiresAttribute requiresAttribute)
                    {
                        // Okay - now let's check the RequiresAttribute for any required components:
                        var containsType = false;
                        foreach (var otherComponent in Components)
                        {
                            if (otherComponent.GetType() == requiresAttribute.requiredType)
                            {
                                containsType = true;
                                break;
                            }
                        }
                        if (!containsType) throw new Exception($"Dependency requirements for {component.GetType().Name} are not fully met.");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Add a component to an entity. This will also check for component dependencies.
        /// </summary>
        /// <param name="component">An instance of the desired component to add.</param>
        public virtual void AddComponent(IComponent component)
        {
            component.Parent = this;
            Components.Add(component);

            // Component added - let's check for any missing component dependencies
            CheckComponentDependencies();
        }

        /// <summary>
        /// Retrieve a component of type <typeparamref name="T1"/> from an entity.
        /// </summary>
        /// <typeparam name="T1">The desired type of component.</typeparam>
        /// <returns>The first component of type <typeparamref name="T1"/> from the entity's component list.</returns>
        public virtual T1 GetComponent<T1>()
        {
            var results = Components.FindAll(t => { return t.GetType() == typeof(T1); });
            if (results.Count <= 0)
                throw new Exception("Component doesn't exist on this entity.");

            return (T1)results.First();
        }

        /// <summary>
        /// Check whether this entity has the component of type T1.
        /// </summary>
        /// <typeparam name="T1">The type of the component to check for.</typeparam>
        /// <returns></returns>
        public bool HasComponent<T1>()
        {
            var results = Components.FindAll(t => { return t.GetType() == typeof(T1); });
            return results.Count > 0;
        }

        public virtual void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            var tmpComponentsCopy = new IComponent[Components.Count];
            Components.CopyTo(tmpComponentsCopy); // Allow component list to be changed mid-notify if necessary

            foreach (var component in tmpComponentsCopy)
            {
                component.OnNotify(notifyType, notifyArgs);
            }
        }

        public void Render()
        {
            foreach (var component in Components)
            {
                component.Render();
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var component in Components)
            {
                component.Update(deltaTime);
            }
        }
    }
}
