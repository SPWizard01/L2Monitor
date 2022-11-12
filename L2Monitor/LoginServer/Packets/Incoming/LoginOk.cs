using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using Serilog;
using System.IO;

namespace L2Monitor.LoginServer.Packets.Incoming
{
    public class LoginOk : BasePacket
    {
        public int SessionKey1 { get; private set; }
        public int SessionKey2 { get; private set; }
        public int Unknown1 { get; private set; }
        public int Unknown2 { get; private set; }
        public int Unknown3 { get; private set; }
        public int Unknown4 { get; private set; }
        public LoginOk()
        {

        }
        public LoginOk(MemoryStream memStream, PacketDirection direction) : base(memStream, true, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new LoginOk(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            client.State = ConnectionState.LOGIN_AUTHENTICATED;
            SessionKey1 = ReadInt32();
            SessionKey2 = ReadInt32();
            Unknown1 = ReadInt32();
            Unknown2 = ReadInt32();
            Unknown3 = ReadInt32();
            Unknown4 = ReadInt32();
            WarnOnRemainingData();
        }
    }
}
