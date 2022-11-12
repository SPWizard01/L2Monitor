using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using System.IO;

namespace L2Monitor.GameServer.Packets.Outgoing
{
    public class ProtocolVersion : BasePacket
    {
        public int Version { get; set; }
        public ProtocolVersion()
        {

        }
        public ProtocolVersion(MemoryStream raw, PacketDirection direction) : base(raw, false, direction)
        {

        }
        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new ProtocolVersion(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            Version = ReadInt32();
            //The rest is unknown
        }
    }
}
