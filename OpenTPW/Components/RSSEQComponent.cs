using ECSEngine.Components;
using OpenTPW.RSSEQ;

namespace OpenTPW.Components
{
    public class RSSEQComponent : Component<RSSEQComponent>
    {
        private VM vm; // probably shouldn't create a new VM instance for every ride
        public RSSEQComponent(string pathToRSE)
        {

        }
    }
}
