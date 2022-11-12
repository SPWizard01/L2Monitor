using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using Serilog;
using System.IO;

namespace L2Monitor.LoginServer.Packets.Incoming
{
    public class PlayOk : BasePacket
    {
        public int SessionKey1 { get; private set; }
        public int SessionKey2 { get; private set; }
        public ushort ServerId { get; private set; }
        public PlayOk()
        {

        }
        public PlayOk(MemoryStream memStream, PacketDirection direction) : base(memStream, true,direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new PlayOk(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            SessionKey1 = ReadInt32();
            SessionKey2 = ReadInt32();
            ServerId = ReadByteAsShort();

            WarnOnRemainingData();
        }
    }
}
