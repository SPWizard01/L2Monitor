using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using L2Monitor.Util;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace L2Monitor.LoginServer.Packets.Incoming
{
    public class Init : BasePacket
    {
        public uint SessionId { get; private set; }
        public uint ProtocolVersion { get; private set; }
        public byte[] RSAPublicKey { get; private set; }
        public uint Unknown1 { get; private set; }
        public int Unknown2 { get; private set; }
        public int Unknown3 { get; private set; }
        public int Unknown4 { get; private set; }
        public int Unknown5 { get; private set; }
        public byte[] BlowFishKey { get; private set; }
        public int Unknown6 { get; private set; }
        public byte[] UnknownBytes { get; private set; }
        public int Unknown7 { get; private set; }
        public uint Unknown8 { get; private set; }

        public Init()
        {

        }

        public Init(MemoryStream raw, PacketDirection direction) : base(raw, true, direction)
        {
            
        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new Init(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            SessionId = ReadUInt32();

            ProtocolVersion = ReadUInt32();
            if (ProtocolVersion != 50721)
            {
                LogNewDataWarning(nameof(ProtocolVersion), ProtocolVersion);
            }

            RSAPublicKey = ReadBytes(128);

            //GG related, 0 in new client
            Unknown2 = ReadInt32();
            Unknown3 = ReadInt32();
            Unknown4 = ReadInt32();
            Unknown5 = ReadInt32();

            BlowFishKey = ReadBytes(16);
            Unknown6 = ReadInt32();
            if (Unknown6 != 133123)
            {
                LogNewDataWarning(nameof(Unknown6), Unknown6);
            }
            UnknownBytes = ReadBytes(11);
            Unknown7 = ReadInt32();
            if (Unknown7 != 0)
            {
                LogNewDataWarning(nameof(Unknown7), Unknown7);
            }
            Unknown8 = ReadUInt32();
            WarnOnRemainingData();

            client.Crypt?.SetKey(BlowFishKey);
            client.State = ConnectionState.LOGIN_KEY_INITED;
            baseLogger.Information(JsonSerializer.Serialize(this));
        }

    }
}
