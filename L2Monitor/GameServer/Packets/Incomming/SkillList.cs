using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using System.IO;
using System.Text.Json;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class SkillList : BasePacket
    {
        public uint SkillCount { get; private set; }
        public SkillList()
        {

        }
        public SkillList(MemoryStream memStream, PacketDirection direction) : base(memStream, false, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new SkillList(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            client.State = ConnectionState.IN_GAME;
            SkillCount = ReadUInt32();
            WarnOnRemainingData();
            baseLogger.Information("{data}", JsonSerializer.Serialize(this));
        }
    }
}
