using Engine.Utils.Base;
using Engine.Utils.FileUtils.FileSystems;

namespace Engine.Utils
{
    public class LocatableService<T>
    {
        private T service;

        public LocatableService(T defaultService)
        {
            service = defaultService;
        }

        public void ProvideService(T service) 
        {
            this.service = service;
        }

        public T GetService()
        {
            return service;
        }
    }

    public static class ServiceLocator
    {
        public static LocatableService<IRenderer> renderer = new LocatableService<IRenderer>(new NullRenderer());
        public static LocatableService<IFileSystem> fileSystem = new LocatableService<IFileSystem>(new NullFileSystem());

        public static IRenderer Renderer => renderer.GetService();
        public static IFileSystem FileSystem => fileSystem.GetService();
    }
}
