using Engine.Utils.MathUtils;
using OpenTPW.Files.FileFormats;

namespace OpenTPW.Entities.UI
{
    public sealed class TGAImageEntity : BaseImageEntity
    {
        public TGAImageEntity(string path, Vector2d position, Vector2d scale) : base(position, scale)
        {
            // Load image
            texture = TGAReader.LoadAsset(path);
            material.diffuseTexture = texture;
        }
    }
}
