using Engine.Utils.MathUtils;
using OpenTPW.Files.FileFormats;
using OpenTPW.Files.FileFormats.BFWD;
using System;
using System.Linq;

namespace OpenTPW.Entities.UI
{
    public sealed class WCTImageEntity : BaseImageEntity
    {
        public WCTImageEntity(string archivePath, string filePath, Vector2d position, Vector2d scale) : base(position, scale)
        {
            var rideArchive = new BFWDArchive(archivePath);
            var rseFile = rideArchive.files.Last(file => file.name.Equals($"{filePath}\0", StringComparison.OrdinalIgnoreCase));
            // Load image
            texture = WCTReader.LoadAsset(rseFile.data);
            material.diffuseTexture = texture;
        }
    }
}