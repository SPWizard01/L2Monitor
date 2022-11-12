using L2Monitor.Classes;
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
    public class RequestServerList : BasePacket
    {
        public int SessionKey1 { get; private set; }
        public int SessionKey2 { get; private set; }

        public RequestServerList()
        {

        }
        public RequestServerList(MemoryStream memoryStream, PacketDirection direction) : base(memoryStream, true, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new RequestServerList(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            SessionKey1 = ReadInt32();
            SessionKey2 = ReadInt32();

            WarnOnRemainingData();
        }
    }
}
