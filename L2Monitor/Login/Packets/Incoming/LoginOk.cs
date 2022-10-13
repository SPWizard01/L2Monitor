using L2Monitor.Common.Packets;
using Serilog;
using System.IO;

namespace L2Monitor.Login.Packets.Incoming
{
    public class LoginOk : BasePacket
    {
        public byte[] SessionKey1 { get; private set; }
        public byte[] SessionKey2 { get; private set; }
        public int Unknown1 { get; private set; }
        public int Unknown2 { get; private set; }
        public int Unknown3 { get; private set; }
        public int Unknown4 { get; private set; }
        public LoginOk(MemoryStream memStream) : base(memStream)
        {
            SessionKey1 = readBytes(4);
            SessionKey2 = readBytes(4);
            Unknown1 = readInt();
            Unknown2 = readInt();
            Unknown3 = readInt();
            Unknown4 = readInt();
            WarnOnRemainingData();
        }
    }
}
