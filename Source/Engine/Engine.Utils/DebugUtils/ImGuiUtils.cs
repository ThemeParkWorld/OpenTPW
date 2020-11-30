using Engine.Types;
using Engine.Utils.Attributes;
using ImGuiNET;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Engine.Utils.DebugUtils
{
    public static class ImGuiUtils
    {
        /// <summary>
        /// Queues a DragFloat3 ImGui component for the Vertex3f parameter given.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="reference"></param>
        public static void DrawImGuiVertex3f(MemberInfo member, object reference)
        {
            var tmpReference = reference as ReflectionRef<Vertex3f>;
            Vector3 value = new Vector3(tmpReference.Value.x, tmpReference.Value.y, tmpReference.Value.z);
            ImGui.DragFloat3(member.Name, ref value, 0.1f);
            (reference as ReflectionRef<Vertex3f>).Value = new Vertex3f(value.X, value.Y, value.Z);
        }

        public static void DrawImGuiVertex2f(MemberInfo member, object reference)
        {
            var tmpReference = reference as ReflectionRef<Vertex2f>;
            Vector2 value = new Vector2(tmpReference.Value.x, tmpReference.Value.y);
            ImGui.DragFloat2(member.Name, ref value, 0.1f);
            (reference as ReflectionRef<Vertex2f>).Value = new Vertex2f(value.X, value.Y);
        }

        public static void DrawImGuiFloat(MemberInfo member, object reference)
        {
            float value = (reference as ReflectionRef<float>).Value;
            var min = float.MinValue;
            var max = float.MaxValue;
            var useSlider = false;
            var fieldAttributes = member.GetCustomAttributes(false);
            foreach (var attrib in fieldAttributes.Where(o => o is RangeAttribute))
            {
                var rangeAttrib = (RangeAttribute)attrib;
                min = rangeAttrib.Min;
                max = rangeAttrib.Max;
                useSlider = true;
            }

            if (useSlider)
                ImGui.SliderFloat($"{member.Name}", ref value, min, max);
            else
                ImGui.InputFloat($"{member.Name}", ref value);
            (reference as ReflectionRef<float>).Value = value;
        }

        public static void DrawImGuiColor(MemberInfo member, object reference)
        {
            var tmpReference = reference as ReflectionRef<ColorRGB24>;
            var value = new Vector3(tmpReference.Value.r / 255f, tmpReference.Value.g / 255f, tmpReference.Value.b / 255f);
            ImGui.ColorEdit3($"{member.Name}", ref value);
            (reference as ReflectionRef<ColorRGB24>).Value = new ColorRGB24((byte)(value.X * 255f), (byte)(value.Y * 255f), (byte)(value.Z * 255f));
        }

        public static void DrawImGuiVector3(MemberInfo member, object reference)
        {
            Vector3 value = (reference as ReflectionRef<Utils.MathUtils.Vector3f>).Value.ConvertToNumerics();
            ImGui.DragFloat3(member.Name, ref value, 0.1f);
            (reference as ReflectionRef<Utils.MathUtils.Vector3f>).Value = Utils.MathUtils.Vector3f.ConvertFromNumerics(value);
        }

        public static void DrawImGuiVector3d(MemberInfo member, object reference)
        {
            Vector3 value = (reference as ReflectionRef<Utils.MathUtils.Vector3d>).Value.ConvertToNumerics();
            ImGui.DragFloat3(member.Name, ref value, 0.1f);
            (reference as ReflectionRef<Utils.MathUtils.Vector3d>).Value = Utils.MathUtils.Vector3d.ConvertFromNumerics(value);
        }

        public static void DrawImGuiVector2(MemberInfo member, object reference)
        {
            Vector2 value = (reference as ReflectionRef<Utils.MathUtils.Vector2f>).Value.ConvertToNumerics();
            ImGui.DragFloat2(member.Name, ref value, 0.1f);
            if (value != (reference as ReflectionRef<Utils.MathUtils.Vector2f>).Value.ConvertToNumerics())
                (reference as ReflectionRef<Utils.MathUtils.Vector2f>).Value = Utils.MathUtils.Vector2f.ConvertFromNumerics(value);
        }

        public static void DrawImGuiQuaternion(MemberInfo member, object reference)
        {
            Vector3 value = (reference as ReflectionRef<Utils.MathUtils.Quaternion>).Value.ToEulerAngles().ConvertToNumerics();
            ImGui.DragFloat3(member.Name, ref value, 0.1f);
            (reference as ReflectionRef<Utils.MathUtils.Quaternion>).Value = Utils.MathUtils.Quaternion.FromEulerAngles(Utils.MathUtils.Vector3f.ConvertFromNumerics(value));
        }

        public static void DrawImGuiInt(MemberInfo field, object reference)
        {
            int value = (reference as ReflectionRef<int>).Value;
            ImGui.DragInt($"{field.Name}", ref value);
            (reference as ReflectionRef<int>).Value = value;
        }

        public static void DrawImGuiArray(dynamic memberValue, int depth)
        {
            foreach (var element in memberValue)
            {
                RenderImGuiMembers(element, depth + 1);
            }
        }

        /// <summary>
        /// Handles the rendering of a Type's member through ImGui, allowing for prototyping.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="depth"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void RenderImGuiMember(object obj, MemberInfo memberInfo, ref int depth)
        {
            Type type;
            dynamic memberValue;
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    memberValue = ((FieldInfo)memberInfo).GetValue(obj);
                    type = ((FieldInfo)memberInfo).FieldType;
                    break;
                case MemberTypes.Property:
                    memberValue = ((PropertyInfo)memberInfo).GetValue(obj);
                    type = ((PropertyInfo)memberInfo).PropertyType;
                    break;
                default:
                    throw new NotImplementedException($"Member type {memberInfo.MemberType} not implemented");
            }

            var referenceType = typeof(ReflectionRef<>).MakeGenericType(type);
            var reference = Activator.CreateInstance(referenceType, memberInfo, obj);

            if (Attribute.IsDefined(memberInfo, typeof(HideInImGuiAttribute)))
            {
                return;
            }

            if (type == typeof(float))
            {
                DrawImGuiFloat(memberInfo, reference);
            }
            else if (type == typeof(ColorRGB24))
            {
                DrawImGuiColor(memberInfo, reference);
            }
            else if (type == typeof(Utils.MathUtils.Vector2f))
            {
                DrawImGuiVector2(memberInfo, reference);
            }
            else if (type == typeof(Vertex2f))
            {
                DrawImGuiVertex2f(memberInfo, reference);
            }
            else if (type == typeof(Utils.MathUtils.Vector3f))
            {
                DrawImGuiVector3(memberInfo, reference);
            }
            else if (type == typeof(Utils.MathUtils.Vector3d))
            {
                DrawImGuiVector3d(memberInfo, reference);
            }
            else if (type == typeof(Vertex3f))
            {
                DrawImGuiVertex3f(memberInfo, reference);
            }
            else if (type == typeof(MathUtils.Quaternion))
            {
                DrawImGuiQuaternion(memberInfo, reference);
            }
            else if (type == typeof(int))
            {
                DrawImGuiInt(memberInfo, reference);
            }
            else if (type == typeof(List<>) || type.BaseType == typeof(Array))
            {
                DrawImGuiArray(memberValue, depth);
            }
            else
            {
                ImGui.LabelText($"{memberInfo.Name}", $"{memberValue}");
            }
        }

        /// <summary>
        /// Get all fields via reflection for use within ImGUI.
        /// </summary>
        /// <param name="depth">The depth of the current reflection.</param>
        public static void RenderImGuiMembers(object obj, int depth = 0)
        {
            if (depth > 1) return; // Prevent any dumb stack overflow errors

            foreach (var field in obj.GetType().GetFields())
            {
                RenderImGuiMember(obj, field, ref depth);
            }
            foreach (var property in obj.GetType().GetProperties())
            {
                RenderImGuiMember(obj, property, ref depth);
            }
        }
    }
}
