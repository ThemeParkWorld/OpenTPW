using System;
using System.Reflection;

namespace Engine.Types
{
    public class ReflectionRef<T>
    {
        private readonly MemberInfo memberInfo;
        private readonly object origin;

        public bool CanGet => memberInfo.MemberType == MemberTypes.Field || (memberInfo.MemberType == MemberTypes.Property && ((PropertyInfo)memberInfo).GetMethod != null);
        public bool CanSet => memberInfo.MemberType == MemberTypes.Field || (memberInfo.MemberType == MemberTypes.Property && ((PropertyInfo)memberInfo).SetMethod != null);

        public T Value
        {
            get
            {
                if (!CanGet)
                    return default;

                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        return (T)((FieldInfo)memberInfo).GetValue(origin);
                    case MemberTypes.Property:
                        var propertyInfo = ((PropertyInfo)memberInfo);
                        return (T)propertyInfo.GetValue(origin);
                }
                throw new NotImplementedException($"Member type {memberInfo.MemberType} not implemented");
            }
            set
            {
                if (!CanSet)
                    return;

                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        ((FieldInfo)memberInfo).SetValue(origin, value);
                        break;
                    case MemberTypes.Property: // TODO: Check if there is no set method beforehand, set as readonly within imgui
                        var propertyInfo = ((PropertyInfo)memberInfo);
                            propertyInfo.SetValue(origin, value);
                        break;
                    default:
                        throw new NotImplementedException($"Member type {memberInfo.MemberType} not implemented");
                }
            }
        }

        public ReflectionRef(MemberInfo memberInfo, object origin)
        {
            this.memberInfo = memberInfo;
            this.origin = origin;
        }
    }
}
