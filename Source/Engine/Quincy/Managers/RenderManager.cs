using Engine.ECS.Managers;
using Engine.Utils;
using Engine.Utils.Base;
using System;
using System.Threading;

namespace Quincy.Managers
{
    public class RenderManager : Manager<RenderManager>
    {
        public string hdri = "HDRIs/studio_small_03_4k.hdr";
        public float exposure = 1.75f;
        public TonemapOperator tonemapOperator;

        public enum TonemapOperator
        {
            None,
            Reinhard,
            ReinhardExtendedLuminance,
            ReinhardJodie,
            AcesApproximation
        };

        private IRenderer renderer;

        private DateTime lastRender;
        private int currentFrametimeIndex;
        private int currentFramerateIndex;
        private const int FramesToCount = 480;
        private float framerateLimitAsMs = 1000f / EngineSettings.FramerateLimit;
        public float LastFrameTime { get; private set; }
        public int CalculatedFramerate => (int)(1000f / Math.Max(LastFrameTime, 0.001f));
        public float[] FrametimeHistory { get; } = new float[FramesToCount];
        public float[] FramerateHistory { get; } = new float[FramesToCount];

        public bool RenderShadowMap { get; set; }
        public bool Paused { get; set; }

        public RenderManager()
        {
            renderer = ServiceLocator.renderer.GetService();
            renderer.ContextCreated();
        }

        /// <summary>
        /// Render all the entities within the render manager.
        /// </summary>
        public override void Run()
        {
            renderer.RenderToShadowMap();
            renderer.RenderToScreen();

            CollectPerformanceData();
        }

        public void CollectPerformanceData()
        {
            LastFrameTime = (DateTime.Now - lastRender).Milliseconds;

            if (!Paused)
            {
                FrametimeHistory[currentFrametimeIndex++] = LastFrameTime;

                if (currentFrametimeIndex == FrametimeHistory.Length)
                {
                    currentFrametimeIndex--;
                    for (var i = 0; i < FrametimeHistory.Length; ++i)
                        FrametimeHistory[i] = FrametimeHistory[(i + 1) % FrametimeHistory.Length];
                }

                FramerateHistory[currentFramerateIndex++] = CalculatedFramerate;
                if (currentFramerateIndex == FramerateHistory.Length)
                {
                    currentFramerateIndex--;
                    for (var i = 0; i < FramerateHistory.Length; ++i)
                        FramerateHistory[i] = FramerateHistory[(i + 1) % FramerateHistory.Length];
                }
            }

            lastRender = DateTime.Now;

            // Slow down rendering if it's going past the framerate limit
            if (LastFrameTime < framerateLimitAsMs && EngineSettings.FramerateLimit > 0)
            {
                var nextFrameDelay = (int)Math.Ceiling(framerateLimitAsMs - LastFrameTime);
                Thread.Sleep(nextFrameDelay);
            }
        }
    }
}
