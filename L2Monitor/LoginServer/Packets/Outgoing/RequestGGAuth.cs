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
    public class RequestGGAuth : BasePacket
    {
        public uint SessionId { get; private set; }
        public RequestGGAuth()
        {

        }

        public RequestGGAuth(MemoryStream memoryStream, PacketDirection direction) : base(memoryStream, true, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new RequestGGAuth(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            SessionId = ReadUInt32();

            WarnOnRemainingData();
        }
    }
}
