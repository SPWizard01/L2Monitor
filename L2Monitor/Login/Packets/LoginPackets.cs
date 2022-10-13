using L2Monitor.Common.Packets;
using L2Monitor.Login.Packets.Incoming;
using L2Monitor.Login.Packets.Outgoing;
using System.Collections.Generic;

namespace L2Monitor.Login.Packets
{
    public static class LoginPackets
    {
        public static List<InListPacket> ClientToServerPackets { get; set; } = new List<InListPacket>
        {
            InListPacket.FromPacketId<RequestGGAuth>(0x07),
            InListPacket.FromPacketId<RequestAuthLogin>(0x0B),
            InListPacket.FromPacketId<RequestServerList>(0x05),
            InListPacket.FromPacketId<RequestServerLogin>(0x02),
        };

        public static List<InListPacket> ServerToClientPackets { get; set; } = new List<InListPacket>
        {
            InListPacket.FromPacketId<Init>(0x00),
            InListPacket.FromPacketId<GGAuth>(0x0B),
            InListPacket.FromPacketId<LoginOk>(0x03),
            InListPacket.FromPacketId<LoginFail>(0x01),
            InListPacket.FromPacketId<ServerList>(0x04),
            InListPacket.FromPacketId<PlayOk>(0x07),
            //

        };
    }
}
