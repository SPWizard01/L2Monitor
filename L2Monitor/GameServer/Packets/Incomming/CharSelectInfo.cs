using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using L2Monitor.GameServer.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class CharSelectInfo : BasePacket
    {
        public int CharCount { get; set; }
        public int MaxCharsInAccount { get; set; }
        public bool CharCreationDisabled { get; set; }

        /// <summary>
        /// 0=can't play, 1=can play free until level 85, 2=100% free play
        /// </summary>
        public byte PlayType { get; set; }
        /// <summary>
        /// if 1, Korean client
        /// </summary>
        public int ClientType { get; set; }

        /// <summary>
        /// Gift message for inactive accounts // 152
        /// </summary>
        public byte Unknown1 { get; set; }

        public bool IsPremium { get; set; }

        public CharSelectInfo()
        {

        }


        public List<CharacterInSelectScreen> Chars { get; set; }
        public CharSelectInfo(MemoryStream raw, PacketDirection direction) : base(raw, false, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new CharSelectInfo(new MemoryStream(raw), direction);
        }
        public override void Run(IL2Client client)
        {
            client.State = ConnectionState.GAME_LOBBY;
            Chars = new List<CharacterInSelectScreen>();
            CharCount = ReadInt32();
            MaxCharsInAccount = ReadInt32(); // should always be 7
            CharCreationDisabled = ReadBoolean();
            PlayType = ReadByte();
            ClientType = ReadInt32();
            Unknown1 = ReadByte();
            IsPremium = ReadBoolean();
            for (int i = 0; i < CharCount; i++)
            {
                Chars.Add(new CharacterInSelectScreen(this));
            }
            WarnOnRemainingData();
            baseLogger.Information(JsonSerializer.Serialize(Chars));
        }

    }
}
