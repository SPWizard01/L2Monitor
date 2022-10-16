using L2Monitor.Common.Packets;
using System.IO;
using System.Text.Json;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class ACopyPacket : BasePacket
    {
        public ACopyPacket(MemoryStream memStream) : base(memStream)
        {
            WarnOnRemainingData();
            baseLogger.Information("{data}", JsonSerializer.Serialize(this));
        }




    }
}
