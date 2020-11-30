using System;

namespace Engine.ECS.Observer
{
    /// <summary>
    /// An interface from which all notification arguments must derive.
    /// </summary>
    public interface INotifyArgs
    {
        /// <summary>
        /// The object triggering the notification.
        /// </summary>
        object Sender { get; set; }

        /// <summary>
        /// The time at which the notification was broadcast.
        /// </summary>
        DateTime TimeSent { get; set; }
    }
}
