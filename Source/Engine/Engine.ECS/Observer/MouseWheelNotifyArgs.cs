using System;

namespace Engine.ECS.Observer
{
    public class MouseWheelNotifyArgs : INotifyArgs
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
        /// The amount by which the mouse scroll wheel has been turned.
        /// </summary>
        public int MouseScroll { get; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseWheelNotifyArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="mouseScroll">The amount by which the mouse scroll wheel has been turned.</param>
        /// <param name="sender">The object triggering the notification.</param>
        public MouseWheelNotifyArgs(int mouseScroll, object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
            MouseScroll = mouseScroll;
        }
    }
}