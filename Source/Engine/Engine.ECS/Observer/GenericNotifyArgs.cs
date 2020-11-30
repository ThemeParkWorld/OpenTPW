using System;

namespace Engine.ECS.Observer
{
    /// <summary>
    /// Generic notification arguments for use when specific parameters are not required.
    /// </summary>
    public class GenericNotifyArgs : INotifyArgs
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
        /// Construct a new instance of <see cref="GenericNotifyArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="sender">The object triggering the notification.</param>
        public GenericNotifyArgs(object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
        }
    }
}
