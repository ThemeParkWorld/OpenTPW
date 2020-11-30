namespace Engine.ECS.Observer
{
    /// <summary>
    /// An enumerator containing all possible events that may occur within a game instance.
    /// </summary>
    public enum NotifyType
    {
        ContextReady,
        GameEnd,

        KeyUp,
        KeyDown,

        MouseMove,
        MouseButtonUp,
        MouseButtonDown,
        MouseScroll,

        WindowResized,
        CloseGame
    }
}
