using System;

namespace Engine.Utils.DebugUtils
{
    public struct DebugResult
    {
        public DebugResultStatus Status { get; set; }
        public Tuple<Type, object>[] Values { get; set; }

        public DebugResult(DebugResultStatus status, params Tuple<Type, object>[] values)
        {
            Status = status;
            Values = values;
        }

        public DebugResult(DebugResultStatus status)
        {
            Status = status;
            Values = null;
        }
    }
}
