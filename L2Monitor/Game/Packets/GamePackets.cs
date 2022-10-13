using L2Monitor.Common.Packets;
using L2Monitor.Game.Packets.Incomming;
using L2Monitor.Game.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Text;

namespace L2Monitor.Game.Packets
{
    public static class GamePackets
    {
        public static List<InListPacket> ClientToServerPackets { get; set; } = new List<InListPacket>
        {
            InListPacket.FromPacketId<ProtocolVersion>(0x00),
            //InListPacket.FromPacketId<AuthLogin>(0x2B)
        };

        public static List<InListPacket> ServerToClientPackets { get; set; } = new List<InListPacket>
        {
            //probably queue position etc..
            //	EX_QUEUE_TICKET_LOGIN(0xFE, 0x1B3),
            //	EX_QUEUE_TICKET(0xFE, 0x1AD),
            //InListPacket.FromPacketId<LoginFailInfo>(0x0A),
            InListPacket.FromPacketId<CryptInit>(0x2E),
            //InListPacket.FromPacketId<CharSelectInfo>(0x09),
            //InListPacket.FromPacketId<SocialAction>(0x27),
            //InListPacket.FromPacketId<CastleStateInfo>(0xFE, 0x12D),
            //

        };
    }
}
