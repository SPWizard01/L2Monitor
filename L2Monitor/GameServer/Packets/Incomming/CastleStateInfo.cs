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
        public CastleStateInfo(MemoryStream memStream) : base(memStream)
        {
            CastleId = readInt();
            CastleState = (CastleState)readInt();
            var a = 1;
        }

        public int CastleId { get; }
        public CastleState CastleState { get; private set; }


    }
}
