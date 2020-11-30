using Engine.Utils.MathUtils;
using OpenTPW.Files;
using OpenTPW.Files.FileFormats;

namespace OpenTPW.Entities.UI
{
    public sealed class TGAImageEntity : BaseImageEntity
    {
        public TGAImageEntity(string path, Vector2d position, Vector2d scale) : base(position, scale)
        {
            SetupMaterial();

            // Load image
            texture = FileManager.Instance.ReadFile(path).Data as Texture2D;
            material.diffuseTexture = texture;
        }
    }
}
