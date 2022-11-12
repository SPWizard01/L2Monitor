using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class Say2 : BasePacket
    {
        public uint RequestId { get; private set; }
        public uint Type { get; private set; }
        public uint Unknown1 { get; private set; }
        public string Actor { get; private set; }
        public int Unknown2 { get; private set; }
        public string Msg { get; private set; }
        public Say2()
        {

        }

        public Say2(MemoryStream memStream, PacketDirection direction) : base(memStream, false, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new Say2(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            RequestId = ReadUInt32();
            Type = ReadUInt16();
            Unknown1 = ReadUInt16();
            Actor = ReadString();
            Unknown2 = ReadInt32();
            Msg = ReadString();
            WarnOnRemainingData();
            baseLogger.Information("Chat: {data}", JsonSerializer.Serialize(this));
        }
    }
}
