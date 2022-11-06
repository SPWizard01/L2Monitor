using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.IO;

namespace L2Monitor.LoginServer.Packets.Incoming
{
    public class LoginFail : BasePacket
    {

        public LoginFail(MemoryStream memStream) : base(memStream)
        {
            baseLogger.Information(BitConverter.ToString(memStream.ToArray()));
            //WarnOnRemainingData();
        }
    }
}
