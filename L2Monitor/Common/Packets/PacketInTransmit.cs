using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Common.Packets
{
    public class PacketInTransmit
    {
        public PacketDirection Direction { get; set; }
        public byte[] PacketData { get; set; }
    }
}
