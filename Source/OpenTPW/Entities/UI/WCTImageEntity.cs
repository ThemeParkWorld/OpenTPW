using Engine.Utils.MathUtils;
using OpenTPW.Files;

namespace OpenTPW.Entities.UI
{
    public sealed class WCTImageEntity : BaseImageEntity
    {
        public WCTImageEntity(string archivePath, string filePath, Vector2d position, Vector2d scale) : base(position, scale)
        {
            SetupMaterial();

            texture = FileManager.Instance.ReadFile(archivePath, filePath).Data as Texture2D;
            material.diffuseTexture = texture;
        }
    }
}