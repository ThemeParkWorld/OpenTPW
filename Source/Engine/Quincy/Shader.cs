using OpenGL;
using Quincy.DebugUtils;
using Quincy.MathUtils;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Quincy
{
    internal class Shader
    {
        private List<string> knownMissingVariables = new List<string>();
        public uint Id { get; set; }

        public Shader(string fragGlslPath, string vertGlslPath)
        {
            var fragGlslContents = File.ReadAllText(fragGlslPath);
            var vertGlslContents = File.ReadAllText(vertGlslPath);

            var fragId = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragId, new[] { fragGlslContents });
            Gl.CompileShader(fragId);

            CheckForErrors(fragId);

            var vertId = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vertId, new[] { vertGlslContents });
            Gl.CompileShader(vertId);

            CheckForErrors(vertId);

            Id = Gl.CreateProgram();
            Gl.AttachShader(Id, fragId);
            Gl.AttachShader(Id, vertId);
            Gl.LinkProgram(Id);

            Gl.DeleteShader(fragId);
            Gl.DeleteShader(vertId);
        }

        public void Use()
        {
            Gl.UseProgram(Id);
        }

        public void SetFloat(string name, float value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform1(Id, loc, value);
            }
        }

        public void SetInt(string name, int value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform1(Id, loc, value);
            }
        }

        public void SetMatrix(string name, Matrix4x4f value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniformMatrix4f(Id, loc, 1, false, value);
            }
        }

        public void SetVector3(string name, Vector3f value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform3(Id, loc, value.x, value.y, value.z);
            }
        }

        internal void SetBool(string name, bool value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform1(Id, loc, value ? 1 : 0);
            }
        }

        private bool GetUniformLocation(string name, out int loc)
        {
            loc = Gl.GetUniformLocation(Id, name);
            if (loc < 0)
            {
                if (knownMissingVariables.Contains(name))
                {
                    return false;
                }

                Logging.Log($"No variable {name}", Logging.Severity.Medium);
                knownMissingVariables.Add(name);
                return false;
            }

            return true;
        }

        private void CheckForErrors(uint shader)
        {
            Gl.GetShader(shader, ShaderParameterName.CompileStatus, out int isCompiled);
            if (isCompiled == 0)
            {
                Gl.GetShader(shader, ShaderParameterName.InfoLogLength, out int maxLength);

                var stringBuilder = new StringBuilder(maxLength);
                Gl.GetShaderInfoLog(shader, maxLength, out int _, stringBuilder);

                Logging.Log(stringBuilder.ToString(), Logging.Severity.Fatal);
            }
        }
    }
}