using Engine.ECS.Managers;
using Engine.Types;
using System.Collections.Generic;

namespace Engine.ECS.Observer
{
    public static class Broadcast
    {
        private static List<IManager> managers = new List<IManager>();
        private static IManager gameInstance;

        public static void SetGame(IManager game)
        {
            gameInstance = game;
        }

        public static void AddManager(IManager manager)
        {
            managers.Add(manager);
        }

        public static void Notify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            gameInstance.OnNotify(eventType, notifyArgs);

            foreach (var manager in managers)
            {
                manager.OnNotify(eventType, notifyArgs);
            }
        }
    }
}
