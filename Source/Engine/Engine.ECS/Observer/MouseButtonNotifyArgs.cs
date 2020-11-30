using System;

namespace Engine.ECS.Observer
{
    public class MouseButtonNotifyArgs : INotifyArgs
    {
        /// <summary>
        /// The object triggering the notification.
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// The time at which the notification was broadcast.
        /// </summary>
        public DateTime TimeSent { get; set; }

        /// <summary>
        /// The mouse button relevant to the event.
        /// </summary>
        public int MouseButton { get; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseButtonNotifyArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="mouseButton">The mouse button relevant to the notification.</param>
        /// <param name="sender">The object triggering the notification.</param>
        public MouseButtonNotifyArgs(int mouseButton, object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
            MouseButton = mouseButton;
        }
    }
}