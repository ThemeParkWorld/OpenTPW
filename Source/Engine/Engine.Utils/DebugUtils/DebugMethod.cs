using System;

namespace Engine.Utils.DebugUtils
{
    public abstract class DebugMethod : DebugMember
    {
        public abstract void Invoke();
    }

    public class DebugMethod<T> : DebugMethod
    {
        private Func<T> method;

        public DebugMethod(string name, string description, Func<T> method)
        {
            this.name = name;
            this.description = description;
            this.method = method;
        }

        public override void Invoke() => method.Invoke();
    }
}