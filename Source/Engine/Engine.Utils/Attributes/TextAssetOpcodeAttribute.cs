using System;

namespace Engine.Utils.Attributes
{
    /// <summary>
    /// Inform plaintext asset loaders of various fields with particular opcodes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class TextAssetOpcodeAttribute : Attribute
    {
        /// <summary>
        /// A list of the opcodes that the field can be loaded from.
        /// </summary>
        public string[] opcodes;

        /// <summary>
        /// Whether or not to invert the value (usually 1.0f - value).
        /// Only valid for <see cref="float"/> data.
        /// </summary>
        public bool invertValue;

        /// <summary>
        /// Construct an instance of the TextAssetOpcodeAttribute.
        /// </summary>
        /// <param name="opcodes">A list of the opcodes that the field can be loaded from.</param>
        public TextAssetOpcodeAttribute(params string[] opcodes)
        {
            this.opcodes = opcodes;
        }

        public TextAssetOpcodeAttribute(string opcode, bool invertValue = false)
        {
            opcodes = new[] { opcode };
            this.invertValue = invertValue;
        }
    }
}
