using Engine.Utils.MathUtils;
using OpenTPW.Files;
using Quincy;

namespace OpenTPW.Entities.UI
{
    public sealed class WCTImageEntity : BaseImageEntity
    {
        public WCTImageEntity(string archivePath, string filePath, Vector2d position, Vector2d scale) : base(position, scale)
        {
            texture = FileManager.Instance.ReadFile<Texture>(archivePath, filePath).Data;
        }
    }
}