namespace Engine.Types
{
    /// <summary>
    /// Any class that is able to have a parent.
    /// </summary>
    public interface IHasParent
    {
        IHasParent Parent { get; set; }

        void RenderImGui();
    }
}
