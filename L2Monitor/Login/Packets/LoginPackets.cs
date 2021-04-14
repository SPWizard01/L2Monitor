using L2Monitor.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Login.Packets
{
    public static class LoginPackets
    {
        public static List<InListPacket> ClientToServerPackets { get; set; } = new List<InListPacket>
        {
        };

        public static List<InListPacket> ServerToClientPackets { get; set; } = new List<InListPacket>
        {
            InListPacket.FromPacketId<Init>(0x00),
            //

        };
    }
}
