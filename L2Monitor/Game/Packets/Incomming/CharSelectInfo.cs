using L2Monitor.Common.Packets;
using L2Monitor.Game.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Monitor.Game.Packets.Incomming
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

        public List<CharacterInSelectScreen> Chars { get; set; }
        public CharSelectInfo(MemoryStream raw) : base(raw)
        {
            Chars = new List<CharacterInSelectScreen>();
            CharCount = readInt();
            MaxCharsInAccount = readInt();
            CharCreationDisabled = readBool();
            PlayType = readByte();
            ClientType = readInt();
            Unknown1 = readByte();
            IsPremium = readBool();
            for (int i = 0; i < CharCount; i++)
            {
                Chars.Add(new CharacterInSelectScreen(this));
            }
            var a = 1;
        }


    }
}
