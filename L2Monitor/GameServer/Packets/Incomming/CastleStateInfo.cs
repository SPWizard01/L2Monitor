using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using L2Monitor.GameServer.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class CastleStateInfo : BasePacket
    {

        public int CastleId { get; set; }
        public CastleState CastleState { get; private set; }

        public CastleStateInfo() { }
        public CastleStateInfo(MemoryStream memStream, PacketDirection direction) : base(memStream, false, direction)
        {

        }


        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new CastleStateInfo(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            CastleId = ReadInt32();
            CastleState = (CastleState)ReadInt32();
        }
    }
}
