using Engine.ECS.Components;
using Engine.Utils;
using Engine.Utils.Attributes;
using Engine.Utils.DebugUtils;
using Engine.Utils.FileUtils;
using Engine.Utils.MathUtils;
using Newtonsoft.Json;
using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Quincy.Components
{
    public class ShaderComponent : Component<ShaderComponent>
    {
        private List<string> knownMissingVariables = new List<string>();
        public uint Id { get; set; }
        private Asset fragShaderAsset, vertShaderAsset;

        public ShaderComponent(Asset jsonAsset)
        {
            /* 
             * Example:
             *  {
             *      "fragment": "standard.frag",
             *      "vertex": "standard.vert"
             *  }
             */
            var shaderDescription = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonAsset.AsString());
            var directory = Path.GetDirectoryName(jsonAsset.MountPath);
            fragShaderAsset = ServiceLocator.FileSystem.GetAsset($"{directory}/{shaderDescription["fragment"]}");
            vertShaderAsset = ServiceLocator.FileSystem.GetAsset($"{directory}/{shaderDescription["vertex"]}");
            Load();
        }

        public ShaderComponent(Asset fragShaderAsset, Asset vertShaderAsset)
        {
            this.fragShaderAsset = fragShaderAsset;
            this.vertShaderAsset = vertShaderAsset;
            Load();
        }

        public void Load()
        {
            var fragGlslContents = fragShaderAsset.AsString();
            var vertGlslContents = vertShaderAsset.AsString();

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

        public void SetVector3f(string name, Vector3f value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform3(Id, loc, value.x, value.y, value.z);
            }
        }

        public void SetVector3d(string name, Vector3d value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform3(Id, loc, (float)value.x, (float)value.y, (float)value.z);
            }
        }

        public void SetVector2f(string name, Vector2f value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform2(Id, loc, (float)value.x, (float)value.y);
            }
        }

        public void SetBool(string name, bool value)
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

        //public override void RenderImGui()
        //{
        //    base.RenderImGui(); var shaderNames = new string[shaders.Length];
        //    for (var i = 0; i < shaders.Length; ++i)
        //    {
        //        shaderNames[i] = shaders[i].FileName;
        //    }

        //    if (shaders.Length > 0)
        //    {
        //        if (ImGui.ListBox("", ref currentShaderItem, shaderNames, shaders.Length))
        //        {
        //            currentShaderSource = shaders[currentShaderItem].ShaderSource[0];
        //        }

        //        if (ImGui.Button("Reload shader"))
        //        {
        //            CreateShader();
        //            shaders[currentShaderItem].ReadSourceFromFile();
        //            shaders[currentShaderItem].Compile();
        //            AttachAndLink();
        //        }
        //    }
        //}
    }
}
