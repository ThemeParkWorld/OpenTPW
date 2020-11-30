using Newtonsoft.Json;
using System;

namespace Engine.Utils.DebugUtils
{
    public class DebugVariable<T> : DebugMember
    {
        private readonly Func<T> getter;
        private readonly Action<T> setter;

        [JsonProperty("value")]
        public T Value
        {
            get => getter();
            set => setter(value);
        }

        public DebugVariable(string name, string description, Func<T> getter, Action<T> setter)
        {
            this.name = name;
            this.description = description;
            this.setter = setter;
            this.getter = getter;
        }
    }
}
