using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Login.Packets.Outgoing
{
    public class RequestAuthLogin : BasePacket
    {
        public uint SessionId { get; private set; }

        public RequestAuthLogin(MemoryStream memoryStream): base(memoryStream)
        {

            WarnOnRemainingData();
        }

    }
}
