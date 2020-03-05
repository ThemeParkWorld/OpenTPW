using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTPW.RSSEQ
{
    public class VMObject
    {
        public Dictionary<int, int> Properties { get; } = new Dictionary<int, int>();

        public int @Type { get; }
        public int Id { get; }

        public VMObject(int type, int id)
        {
            Type = type;
            Id = id;
        }
    }
}
