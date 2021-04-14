using L2Monitor.Common.Packets;
using L2Monitor.Util;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Monitor.Login
{
    public class Init : BasePacket
    {
        public int SessionId { get; set; }
        public int ProtocolVersion { get; set; }
        public byte[] RSAPublicKey { get; set; }
        public int Unknown1 { get; private set; }
        public int Unknown2 { get; private set; }
        public int Unknown3 { get; private set; }
        public int Unknown4 { get; private set; }
        public byte[] BlowFishKey { get; set; }

        private readonly ILogger logger;
        public Init(MemoryStream raw) : base(raw)
        {
            logger = Log.ForContext<Init>();
            SessionId = readInt();
            ProtocolVersion = readInt();
            RSAPublicKey = readBytes(128);

            //GG related, 0 in new client
            Unknown1 = readInt();
            Unknown2 = readInt();
            Unknown3 = readInt();
            Unknown4 = readInt();

            BlowFishKey = readBytes(16);

            if(HasRemainingData())
            {
                var data = GetRemainingData();
                logger.Warning($"This packet has remaining data: {BitConverter.ToString(data)}");
            }
        }

        public override void Parse()
        {
            throw new NotImplementedException();
        }
    }
}
