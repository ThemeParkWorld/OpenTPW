using Engine.Utils.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine.Utils.DebugUtils
{
    public static class CommandRegistry
    {
        private static List<DebugMember> debugMembers = new List<DebugMember>();

        private static void RegisterVariable<T>(string name, string description, Func<T> getter, Action<T> setter)
        {
            debugMembers.Add(new DebugVariable<T>(name, description, getter, setter));
        }

        private static void RegisterMethod(string name, string description, Action method)
        {
            debugMembers.Add(new DebugMethod<object>(name, description, () =>
            {
                method.Invoke();
                return null;
            }));
        }

        private static bool FindDebugFunction(string methodName, out DebugMember debugMember)
        {
            var debugCommandMatch = debugMembers.FirstOrDefault(t => t.name == methodName);

            if (debugCommandMatch == null)
            {
                Logging.Log($"No such method {methodName}");
                debugMember = null;
                return false;
            }

            debugMember = debugCommandMatch;
            return true;
        }

        public static DebugResult ExecuteMethod(string methodName)
        {
            if (!FindDebugFunction(methodName, out var debugMember))
                return new DebugResult(DebugResultStatus.Failure);

            throw new NotImplementedException();
            return new DebugResult(DebugResultStatus.Success);
        }

        public static DebugResult ExecuteMethod(string methodName, List<string> parameters)
        {
            if (!FindDebugFunction(methodName, out var debugMember))
                return new DebugResult(DebugResultStatus.Failure);

            throw new NotImplementedException();
            return new DebugResult(DebugResultStatus.Success);
        }

        public static void RegisterAllCommands()
        {
            // TODO: Optimize?
            var start = DateTime.Now;
            foreach (var method in Assembly.GetExecutingAssembly().GetTypes().SelectMany(t => t.GetMethods()).Where(m => m.GetCustomAttributes(typeof(ConsoleFunction), false).Length > 0))
            {
                var consoleFunction = method.GetCustomAttribute<ConsoleFunction>();
                RegisterMethod(consoleFunction.FunctionName, "", () => method.Invoke(null, null));
            }
            var totalTime = (DateTime.Now - start);
            Logging.Log($"RegisterAllCommands took {totalTime.TotalSeconds:F2}s");
        }

        public static List<DebugMember> GiveSuggestions(string input) => (List<DebugMember>)debugMembers.Where(t => t.MatchSuggestion(input)).Take(5);

        public static void ParseAndExecute(string currentConsoleInput)
        {
            /*
             * Command syntax:
             * [function name] <parameters | "parameter with spaces!!">
             */

            List<string> strings = new List<string>();
            StringBuilder currentBuffer = new StringBuilder();
            bool isInString = false;
            foreach (var c in currentConsoleInput)
            {
                switch (c)
                {
                    case '"':
                        isInString = !isInString;
                        continue;
                    case ' ' when !isInString:
                        strings.Add(currentBuffer.ToString());
                        currentBuffer = new StringBuilder();
                        continue;
                }

                currentBuffer.Append(c);
            }

            // Write remaining buffer contents
            strings.Add(currentBuffer.ToString());

            if (strings.Count <= 0)
                return;

            if (strings.Count == 1)
                ExecuteMethod(strings[0]);
            else
                ExecuteMethod(strings[0], strings.GetRange(1, strings.Count - 1));
        }

        #region Debug commands

        [ConsoleFunction("Find")]
        public static string Find(string str)
        {
            List<DebugMember> suggestions = new List<DebugMember>();
            suggestions = (List<DebugMember>)debugMembers.Where(t => t.MatchSuggestion(str));
            return JsonConvert.SerializeObject(suggestions);
        }
        #endregion
    }
}
