using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using Org.BouncyCastle.Crypto.IO;
using Serilog;
using System;
using System.IO;

namespace L2Monitor.LoginServer.Packets.Incoming
{
    public class LoginFail : BasePacket
    {
        public LoginFail()
        {

        }
        public LoginFail(MemoryStream memStream, PacketDirection direction) : base(memStream, true, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new LoginFail(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            baseLogger.Information(BitConverter.ToString(ReadBytes((int)BaseStream.Length)));
            //WarnOnRemainingData();
        }
    }
}
