using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using System.IO;
using System.Text.Json;

namespace L2Monitor.GameServer.Packets.Outgoing
{
    public class PingReply : BasePacket
    {
        public uint RequestId { get; private set; }
        public uint SomeNumber { get; private set; }
        public uint Unknown { get; private set; }
        public PingReply()
        {

        }
        public PingReply(MemoryStream raw, PacketDirection direction) : base(raw, false, direction)
        {

        }
        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new PingReply(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            RequestId = ReadUInt32();
            SomeNumber = ReadUInt32();
            Unknown = ReadUInt32();
            if (Unknown != 3686)
            {
                LogNewDataWarning(nameof(Unknown), 3686, Unknown);
            }
            WarnOnRemainingData();


        }

    }
}
