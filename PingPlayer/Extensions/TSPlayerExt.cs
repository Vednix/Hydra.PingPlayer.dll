using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace PingPlayer.Extensions
{
    public class TSPlayerExt
    {
        public static TSPlayer[] tsArray { get; set; } = TShock.Players;
        public static bool[] Pinged { get; set; } = new bool[Main.maxNetPlayers];
        public static bool[] PingChat { get; set; } = new bool[Main.maxNetPlayers];
        public static bool[] PingStatus { get; set; } = new bool[Main.maxNetPlayers];
        public static bool[] WarnPingOrange { get; set; } = new bool[Main.maxNetPlayers];
        public static bool[] WarnPingRed { get; set; } = new bool[Main.maxNetPlayers];
        internal static void Reset(int index)
        {
            Pinged[index] = false;
            PingChat[index] = false;
            PingStatus[index] = false;
            WarnPingOrange[index] = false;
            WarnPingRed[index] = false;
        }
    }
}
