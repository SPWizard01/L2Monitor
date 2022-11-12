using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using System.IO;

namespace L2Monitor.GameServer.Packets.Outgoing
{
    public class AuthLogin : BasePacket
    {
        public string Login { get; set; }
        public int PlayKey2 { get; set; }
        public int PlayKey1 { get; set; }
        public int LoginKey1 { get; set; }
        public int LoginKey2 { get; set; }
        public AuthLogin()
        {

        }

        public AuthLogin(MemoryStream raw, PacketDirection direction) : base(raw, false, direction)
        {

        }
        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new AuthLogin(new MemoryStream(raw), direction);
        }
        public override void Run(IL2Client client)
        {
            Login = ReadString();
            PlayKey2 = ReadInt32();
            PlayKey1 = ReadInt32();
            LoginKey1 = ReadInt32();
            LoginKey2 = ReadInt32();
        }

    }
}
