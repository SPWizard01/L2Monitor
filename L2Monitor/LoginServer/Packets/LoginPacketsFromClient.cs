using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using L2Monitor.LoginServer.Packets.Outgoing;
using System.Collections.Generic;

namespace L2Monitor.LoginServer.Packets
{
    public static class LoginPacketsFromClient
    {
        public static List<RegisteredPacket> All { get; } = new()
        {
            new RegisteredPacket("AUTH_GAME_GUARD",0x07, new RequestGGAuth(), ConnectionState.LOGIN_KEY_INITED),
            //not used?
            new RegisteredPacket("REQUEST_LOGIN",0x0B, null, ConnectionState.LOGIN_AUTHED_GG),
            new RegisteredPacket("REQUEST_AUTH_LOGIN",0x00, new RequestAuthLogin(), ConnectionState.LOGIN_AUTHED_GG),
            new RegisteredPacket("REQUEST_SERVER_LOGIN",0x02, new RequestServerLogin(), ConnectionState.LOGIN_AUTHENTICATED),
            new RegisteredPacket("REQUEST_SERVER_LIST",0x05, new RequestServerList(), ConnectionState.LOGIN_AUTHENTICATED),
            new RegisteredPacket("REQUEST_PI_AGREEMENT_CHECK",0x0E, null, ConnectionState.LOGIN_AUTHENTICATED),
            new RegisteredPacket("REQUEST_PI_AGREEMENT", 0x0F, null, ConnectionState.LOGIN_AUTHENTICATED),
        };
    }
}
