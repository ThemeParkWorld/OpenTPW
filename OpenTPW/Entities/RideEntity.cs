using System.IO;
using ECSEngine.Entities;
using OpenTPW.Components;

namespace OpenTPW.Entities
{
    public sealed class RideEntity : Entity<RideEntity>
    {
        private string rideName;
        public RideEntity(string rideFolderPath)
        {
            rideName = Path.GetDirectoryName(rideFolderPath);
            AddComponent(new RSSEQComponent($"{rideFolderPath}/{rideName}.RSE"));
        }
    }
}
