using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using System.Net.NetworkInformation;
using PingPlayer.Extensions;

namespace PingPlayer
{
    public class AutoPing
    {
        protected static string BlankLine(int number)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < number; i++)
            {
                sb.Append("\r\n");
            }

            return sb.ToString();
        }
        public static async void Run()
        {
            await Task.Run(async () =>
             {
                 await PingPlayerRun();
             });
        }
        public static async Task PingPlayerRun()
        {
            while (true)
            {
                //SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                //await Task.Factory.StartNew(() => Task.Delay(10000)).ContinueWith((t) =>
                //{
                Parallel.ForEach(TSPlayerExt.tsArray.Where(p => p != null && p.Active && !string.IsNullOrEmpty(p.IP)), player =>
                {
                    if (PlayerPing.Wait)
                        return;
                    Ping p = new Ping();

                    string ping = p.Send(player.IP).RoundtripTime.ToString();
                    if (player.IP == "127.0.0.1" || player.IP.StartsWith("10.0.") || player.IP.StartsWith("192.168.") && ping == "0")
                        ping = "<1";

                    if (ping != "0")
                    {
                        if (ping != "<1" && Int32.Parse(ping) >= 230 && !TSPlayerExt.WarnPingRed[player.Index])
                        {
                            if (player.IsPortuguese)
                                player.SendErrorMessage("Seu tempo de resposta com o servidor está acima de 230ms, lag será notado em diversos momentos.");
                            else
                                player.SendErrorMessage("Your response time with the server is above 230ms, lag will be noticed in several moments.");
                            TSPlayerExt.WarnPingRed[player.Index] = true;
                            TSPlayerExt.Pinged[player.Index] = false;
                        }
                        if (ping != "<1" && Int32.Parse(ping) >= 120 && !TSPlayerExt.WarnPingOrange[player.Index] && !TSPlayerExt.WarnPingRed[player.Index])
                        {
                            if (player.IsPortuguese)
                                player.SendWarningMessage("Seu tempo de resposta com o servidor está acima de 120ms, lag poderá ser notado em alguns momentos.");
                            else
                                player.SendWarningMessage("Your response time with the server is above 120ms, lag may be noticed in a few moments.");
                            TSPlayerExt.WarnPingOrange[player.Index] = true;
                            TSPlayerExt.Pinged[player.Index] = false;
                        }

                        if (!TSPlayerExt.Pinged[player.Index])
                        {
                            player.SendInfoMessage($"AutoPing: {ping}ms");
                            TSPlayerExt.Pinged[player.Index] = true;
                        }

                        int lines = 8;
                        string blanks = BlankLine(2);
                        int bytes = 0;
                        if (!player.Mobile)
                        {
                            lines = 20;
                            blanks = BlankLine(60);
                            bytes = 17;
                        }
                        string message = string.Format($"{BlankLine(lines)}AutoPing: {ping}ms{blanks}");
                        if (TSPlayerExt.PingStatus[player.Index])
                            player.SendData(PacketTypes.Status, message, bytes);
                        if (TSPlayerExt.PingChat[player.Index])
                            player.SendInfoMessage($"AutoPing {ping}ms");
                        p.Dispose();
                    }
                });
                //}, TaskScheduler.FromCurrentSynchronizationContext());
                await Task.Delay(12000);
            }
        }
    }
}
