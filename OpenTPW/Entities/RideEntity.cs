using Engine.ECS.Entities;
using OpenTPW.Components;
using System.IO;

namespace OpenTPW.Entities
{
    public sealed class RideEntity : Entity<RideEntity>
    {
        private string rideName;
        public RideEntity(string rideArchivePath)
        {
            rideName = Path.GetFileNameWithoutExtension(rideArchivePath);
            AddComponent(new RideComponent(rideArchivePath));
        }
    }
}
