using L2Monitor.Common.Packets;
using Serilog;
using System.IO;

namespace L2Monitor.LoginServer.Packets.Incoming
{
    public class PlayOk : BasePacket
    {
        public byte[] SessionKey1 { get; private set; }
        public byte[] SessionKey2 { get; private set; }
        public ushort ServerId { get; private set; }
        public PlayOk(MemoryStream memStream) : base(memStream)
        {
            SessionKey1 = readBytes(4);
            SessionKey2 = readBytes(4);
            ServerId = readByteAsShort();

            WarnOnRemainingData();
        }
    }
}
