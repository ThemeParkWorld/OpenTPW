using System;

namespace Engine.ECS.Observer
{
    public class KeyboardNotifyArgs : INotifyArgs
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
        /// The keyboard key relevant to the notification.
        /// </summary>
        public int KeyboardKey { get; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseWheelNotifyArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="keyboardKey">The keyboard key relevant to the notification.</param>
        /// <param name="sender">The object triggering the notification.</param>
        public KeyboardNotifyArgs(int keyboardKey, object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
            KeyboardKey = keyboardKey;
        }
    }
}