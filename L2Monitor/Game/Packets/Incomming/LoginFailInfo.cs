using L2Monitor.Common.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Game.Packets.Incomming
{
    public class LoginFailInfo : BasePacket
    {
        public LoginFailInfo(MemoryStream memStream) : base(memStream)
        {
            Failed = readInt();

            Reason = readInt();
        }

        /// <summary>
        /// if -1 its a success
        /// </summary>
        public int Failed { get; private set; }
        /// <summary>
        /// reason id displayed in client
        /// </summary>
        public int Reason { get; private set; }

        public override void Parse()
        {
            throw new NotImplementedException();
        }
    }
}
