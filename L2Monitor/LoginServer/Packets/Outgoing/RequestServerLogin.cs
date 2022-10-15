using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.LoginServer.Packets.Outgoing
{
    public class RequestServerLogin : BasePacket
    {
        public byte[] SessionKey1 { get; private set; }
        public byte[] SessionKey2 { get; private set; }
        public ushort ServerId { get; private set; }

        public RequestServerLogin(MemoryStream memoryStream) : base(memoryStream)
        {
            SessionKey1 = readBytes(4);
            SessionKey2 = readBytes(4);
            ServerId = readByteAsShort();

            WarnOnRemainingData();
        }
    }
}
