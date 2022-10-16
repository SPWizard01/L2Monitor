using L2Monitor.Common.Packets;
using System.IO;
using System.Text.Json;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class SkillList : BasePacket
    {
        public uint SkillCount { get; private set; }
        public SkillList(MemoryStream memStream) : base(memStream)
        {
            SkillCount = readUInt();
            WarnOnRemainingData();
            baseLogger.Information("{data}", JsonSerializer.Serialize(this));
        }




    }
}
