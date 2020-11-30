using Engine.ECS.Managers;
using System.Collections.Generic;

namespace Engine.ECS.Observer
{
    public static class Broadcast
    {
        private static List<IManager> managers = new List<IManager>();

        public static void AddManager(IManager manager)
        {
            managers.Add(manager);
        }

        public static void Notify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            foreach (var manager in managers)
            {
                manager.OnNotify(eventType, notifyArgs);
            }
        }
    }
}
