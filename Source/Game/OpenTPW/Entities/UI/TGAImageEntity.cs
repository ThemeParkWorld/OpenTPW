using Engine.Utils.MathUtils;
using OpenTPW.Files;
using Quincy;

namespace OpenTPW.Entities.UI
{
    public sealed class TGAImageEntity : BaseImageEntity
    {
        public TGAImageEntity(string path, Vector2d position, Vector2d scale) : base(position, scale)
        {
            texture = FileManager.Instance.ReadFile<Texture>(path).Data;
        }
    }
}
