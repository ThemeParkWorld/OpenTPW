using OpenGL;
using Quincy.MathUtils;
using Quincy.Primitives;
using System;

namespace Quincy
{
    internal class Scene
    {
        private Model testModel;
        private Shader shader, depthShader;
        private Camera camera;
        private Light light;

        private Framebuffer mainFramebuffer;
        private Plane framebufferRenderPlane;
        private Shader framebufferRenderShader;

        private DateTime lastUpdate;

        private Cubemap skybox;
        private Cubemap convolutedSkybox;
        private Cubemap prefilteredSkybox;

        private Texture brdfLut;
        private Texture holoTexture;

        private Shader skyboxShader;
        private Cube skyboxCube;

        public Scene()
        {
            testModel = new Model(Constants.model);
            shader = new Shader("Content/Shaders/PBR/pbr.frag", "Content/Shaders/PBR/pbr.vert");
            depthShader = new Shader("Content/Shaders/Depth/depth.frag", "Content/Shaders/Depth/depth.vert");
            camera = new Camera(position: new Vector3f(0, 0f, 1f));
            light = new Light(position: new Vector3f(0, 10f, 0));

            framebufferRenderShader = new Shader("Content/Shaders/Framebuffer/framebuffer.frag", "Content/Shaders/Framebuffer/framebuffer.vert");
            framebufferRenderPlane = new Plane();
            mainFramebuffer = new Framebuffer();

            var cubemaps = EquirectangularToCubemap.Convert(Constants.hdri);
            skybox = cubemaps.Item1;
            convolutedSkybox = cubemaps.Item2;
            prefilteredSkybox = cubemaps.Item3;

            brdfLut = new Texture()
            {
                Id = EquirectangularToCubemap.CreateBrdfLut(),
                Path = "brdfLut",
                Type = "texture_lut"
            };

            holoTexture = Texture.LoadFromFile("Content/Textures/holoMap.png", "texture_diffuse");

            skyboxShader = new Shader("Content/Shaders/Skybox/skybox.frag", "Content/Shaders/Skybox/skybox.vert");
            skyboxCube = new Cube();
        }

        public void Render()
        {
            Update();

            RenderSceneToFramebuffer();
            RenderFramebufferToScreen();
        }

        private void RenderSceneToFramebuffer()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, mainFramebuffer.Fbo);
            Gl.Viewport(0, 0, Constants.renderWidth, Constants.renderHeight);
            Gl.ClearDepth(0.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.ZeroToOne);
            Gl.DepthFunc(DepthFunction.Greater);

            camera.Render();
            DrawSkybox();
            testModel.Draw(camera, shader, light, (skybox, convolutedSkybox, prefilteredSkybox), brdfLut, holoTexture);

            Gl.DepthFunc(DepthFunction.Less);
            Gl.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.NegativeOneToOne);
        }

        private void RenderFramebufferToScreen()
        {
            Gl.Viewport(0, 0, Constants.windowWidth, Constants.windowHeight);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Gl.ClearDepth(1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            framebufferRenderShader.SetFloat("exposure", Constants.exposure);
            framebufferRenderPlane.Draw(framebufferRenderShader, mainFramebuffer.ColorTexture);
        }

        public void RenderShadows()
        {
            Gl.Disable(EnableCap.CullFace);

            Gl.Viewport(0, 0, (int)light.ShadowMap.Resolution.x, (int)light.ShadowMap.Resolution.y);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, light.ShadowMap.DepthMapFbo);
            Gl.Clear(ClearBufferMask.DepthBufferBit);

            light.Render();
            testModel.DrawShadows(light, depthShader);

            Gl.Enable(EnableCap.CullFace);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Update()
        {
            float deltaTime = (float)(DateTime.Now - lastUpdate).TotalSeconds;
            testModel.Update(deltaTime);
            camera.Update(deltaTime);

            lastUpdate = DateTime.Now;
        }

        private void DrawSkybox()
        {
            Gl.Disable(EnableCap.CullFace);
            var modelMatrix = Matrix4x4f.Identity;
            var scale = 10000.0f;
            modelMatrix.Scale(scale, scale, scale);
            skyboxShader.Use();
            skyboxShader.SetMatrix("projMatrix", camera.ProjMatrix);
            skyboxShader.SetMatrix("viewMatrix", camera.ViewMatrix);
            skyboxShader.SetMatrix("modelMatrix", modelMatrix);
            skyboxShader.SetInt("environmentMap", 0);

            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.TextureCubeMap, skybox.Id);

            skyboxCube.Draw();

            Gl.BindTexture(TextureTarget.TextureCubeMap, 0);
            Gl.Enable(EnableCap.CullFace);
        }
    }
}
