using ECSEngine.MathUtils;
using OpenTPW.Files.FileFormats;

namespace OpenTPW.Entities.UI
{
    public sealed class TGAImageEntity : BaseImageEntity
    {
        public TGAImageEntity(string path, Vector2 position, Vector2 scale) : base(position, scale)
        {
            // Load image
            texture = TGAReader.LoadAsset(path);
            material.diffuseTexture = texture;
        }
    }
}
