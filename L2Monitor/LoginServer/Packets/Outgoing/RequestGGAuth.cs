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
    public class RequestGGAuth : BasePacket
    {
        public uint SessionId { get; private set; }

        public RequestGGAuth(MemoryStream memoryStream) : base(memoryStream)
        {
            SessionId = readUInt();

            WarnOnRemainingData();
        }
    }
}
