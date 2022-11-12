using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using L2Monitor.LoginServer.Packets.Incoming;
using System;
using System.Collections.Generic;

namespace L2Monitor.LoginServer.Packets
{
    public static class LoginPacketsFromServer
    {
        public static List<RegisteredPacket> All { get; } = new()
        {
            new RegisteredPacket("INIT", 0x00, new Init(), Enum.GetValues<ConnectionState>()),
            new RegisteredPacket("LOGIN_FAIL", 0x01, new LoginFail(), Enum.GetValues<ConnectionState>()),
            new RegisteredPacket("ACCOUNT_KICKED", 0x02, null, Enum.GetValues<ConnectionState>()),
            new RegisteredPacket("LOGIN_OK", 0x03, new LoginOk(), Enum.GetValues<ConnectionState>()),
            new RegisteredPacket("SERVER_LIST", 0x04, new ServerList(), Enum.GetValues<ConnectionState>()),
            new RegisteredPacket("PLAY_FAIL", 0x06, null, Enum.GetValues<ConnectionState>()),
            new RegisteredPacket("PLAY_OK", 0x07, new PlayOk(), Enum.GetValues<ConnectionState>()),

            new RegisteredPacket("PI_AGREEMENT_CHECK", 0x11, null, Enum.GetValues<ConnectionState>()),
            new RegisteredPacket("PI_AGREEMENT_ACK", 0x12, null, Enum.GetValues<ConnectionState>()),
            new RegisteredPacket("GG_AUTH", 0x0b, new GGAuth(), Enum.GetValues<ConnectionState>()),
            new RegisteredPacket("LOGIN_OPT_FAIL", 0x0D, null, Enum.GetValues<ConnectionState>()),
    };
    }
}
