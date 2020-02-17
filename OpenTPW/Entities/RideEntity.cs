using ECSEngine.Entities;
using OpenTPW.Components;
using System;
using System.IO;
using System.Linq;
using OpenTPW.Files.FileFormats.BFWD;

namespace OpenTPW.Entities
{
    public sealed class RideEntity : Entity<RideEntity>
    {
        private string rideName;
        public RideEntity(string rideArchivePath)
        {
            rideName = Path.GetFileNameWithoutExtension(rideArchivePath);
            var rideArchive = new BFWDArchive(rideArchivePath);
            var rseFile = rideArchive.files.First(file => file.name.Equals($"{rideName}.RSE\0", StringComparison.OrdinalIgnoreCase));
            AddComponent(new RSSEQComponent(rseFile.data));
        }
    }
}
