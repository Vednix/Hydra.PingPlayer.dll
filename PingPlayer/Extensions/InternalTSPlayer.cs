using Hydra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Hidra.PingPlayer.Extensions
{
    class InternalTSPlayer
    {
        public static bool[] Pinged { get; set; } = new bool[Main.maxPlayers];
        public static bool[] PingChat { get; set; } = new bool[Main.maxPlayers];
        public static bool[] PingStatus { get; set; } = new bool[Main.maxPlayers];
        public static bool[] WarnPingOrange { get; set; } = new bool[Main.maxPlayers];
        public static bool[] WarnPingRed { get; set; } = new bool[Main.maxPlayers];
        public static string[] PingedIP { get; set; } = new string[Main.maxPlayers];
        public static void ResetPingStats(int index)
        {
            if (PingedIP[index] == TShockB.Players[index].IP)
                return;
            Pinged[index] = false;
            PingChat[index] = false;
            PingStatus[index] = false;
            WarnPingOrange[index] = false;
            WarnPingRed[index] = false;
        }
    }
}
