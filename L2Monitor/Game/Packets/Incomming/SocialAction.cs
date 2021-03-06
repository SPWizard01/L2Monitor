using L2Monitor.Common.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Game.Packets.Incomming
{
    public class SocialAction : BasePacket
    {
        public SocialAction(MemoryStream memStream) : base(memStream)
        {
            ObjectId = readInt();
            ActionId = readInt();
            Unknown1 = readInt();
        }

        public int ObjectId { get; private set; }
        public int ActionId { get; private set; }
        public int Unknown1 { get; private set; }

        public override void Parse()
        {
            throw new NotImplementedException();
        }
    }
}
