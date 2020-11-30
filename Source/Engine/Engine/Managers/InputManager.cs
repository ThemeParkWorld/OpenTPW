using Engine.ECS.Managers;
using Engine.ECS.Observer;

namespace Engine.Managers
{
    class InputManager : Manager<InputManager>
    {
        public InputManager()
        {
        }

        public override void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            base.OnNotify(notifyType, notifyArgs);
        }

        public override void Run()
        {
            base.Run();
        }
    }
}
