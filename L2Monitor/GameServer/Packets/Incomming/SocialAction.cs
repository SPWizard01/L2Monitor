using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class SocialAction : BasePacket
    {

        public int ObjectId { get; private set; }
        public int ActionId { get; private set; }
        public int Unknown1 { get; private set; }
        public SocialAction()
        {

        }
        public SocialAction(MemoryStream memStream, PacketDirection direction) : base(memStream, false, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new SocialAction(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            ObjectId = ReadInt32();
            ActionId = ReadInt32();
            Unknown1 = ReadInt32();
        }
    }
}
