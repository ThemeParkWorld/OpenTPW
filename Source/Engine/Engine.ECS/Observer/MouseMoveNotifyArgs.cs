using Engine.Utils.MathUtils;
using System;

namespace Engine.ECS.Observer
{
    public class MouseMoveNotifyArgs : INotifyArgs
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
        /// The mouse cursor's new position relative to the window.
        /// </summary>
        public Vector2f MousePosition { get; }

        /// <summary>
        /// The mouse cursor's position relative to the last update.
        /// </summary>
        public Vector2f MouseDelta { get; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseMoveNotifyArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="mouseDelta">The cursor's delta.</param>
        /// <param name="mousePosition">The cursor's new position relative to the window.</param>
        /// <param name="sender">The object triggering the notification.</param>
        public MouseMoveNotifyArgs(Vector2f mouseDelta, Vector2f mousePosition, object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
            MouseDelta = mouseDelta;
            MousePosition = mousePosition;
        }
    }
}
