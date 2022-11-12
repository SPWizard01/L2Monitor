using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Classes
{
    public enum ConnectionState
    {
        CONNECTED,
        LOGIN_KEY_INITED,
        LOGIN_AUTHED_GG,
        LOGIN_AUTHENTICATED,
        LOGIN_ENTERING_GAMESERVER,
        GAME_LOBBY,
        GAME_ENTERING,
        IN_GAME,
        DISCONNECTED,
    }
}
