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
    public class LoginOutcome : BasePacket
    {
        /// <summary>
        /// if -1 its a success
        /// </summary>
        public int Failed { get; private set; }
        /// <summary>
        /// reason id displayed in client
        /// </summary>
        public int Reason { get; private set; }

        public LoginOutcome()
        {

        }

        public LoginOutcome(MemoryStream memStream, PacketDirection direction) : base(memStream, false, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new LoginOutcome(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            Failed = ReadInt32();

            Reason = ReadInt32();
            WarnOnRemainingData();
        }
    }
}
