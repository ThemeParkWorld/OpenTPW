using System;

namespace Engine.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ConsoleFunction : Attribute
    {
        public string FunctionName { get; set; }

        public ConsoleFunction(string functionName)
        {
            FunctionName = functionName;
        }
    }
}
