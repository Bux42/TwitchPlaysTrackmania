using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchPlaysTrackmania
{
    class Settings
    {
        public string ChannelName = null;
        public string Username = null;
        public string OAuth = null;

        public string UpChatCommand = "!up";
        public string DownChatCommand = "!down";
        public string LeftChatCommand = "!left";
        public string RightChatCommand = "!right";

        public int UpPressTime = 100;
        public int DownPressTime = 100;
        public int LeftPressTime = 100;
        public int RightPressTime = 100;
    }
}
