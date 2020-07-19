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
using static Hydra.Extensions.Tools;
using Hydra.Extensions;
using Hydra;

namespace PingPlayer
{
    public class AutoPing
    {
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
                try
                {
                    Parallel.ForEach(TShockB.Players.Where(p => p != null && p.Active && !string.IsNullOrEmpty(p.IP)), player =>
                    {
                        if (PlayerPing.Wait || Hydra.Base.isDisposed)
                            return;
                        Ping p = new Ping();

                        string ping = p.Send(player.IP).RoundtripTime.ToString();
                        if (player.IP == "127.0.0.1" || player.IP.StartsWith("10.0.") || player.IP.StartsWith("192.168.") && ping == "0")
                            ping = "<1";

                        if (ping != "0")
                        {
                            if (ping != "<1" && Int32.Parse(ping) >= 230 && !TSPlayerB.WarnPingRed[player.Index])
                            {
                                TSPlayerB.SendErrorMessage(player.Index, "Your response time with the server is above 230ms, lag will be noticed in several moments.",
                                                                          "Seu tempo de resposta com o servidor está acima de 230ms, lag será notado em diversos momentos.");
                                TSPlayerB.WarnPingRed[player.Index] = true;
                                TSPlayerB.Pinged[player.Index] = false;
                            }
                            if (ping != "<1" && Int32.Parse(ping) >= 120 && !TSPlayerB.WarnPingOrange[player.Index] && !TSPlayerB.WarnPingRed[player.Index])
                            {
                                TSPlayerB.SendWarningMessage(player.Index, "Your response time with the server is above 120ms, lag may be noticed in a few moments.",
                                                                          "Seu tempo de resposta com o servidor está acima de 120ms, lag poderá ser notado em alguns momentos.");
                                TSPlayerB.WarnPingOrange[player.Index] = true;
                                TSPlayerB.Pinged[player.Index] = false;
                            }

                            if (!TSPlayerB.Pinged[player.Index])
                            {
                                player.SendSuccessMessage($"Ping {ping}ms");
                                TSPlayerB.Pinged[player.Index] = true;
                            }

                            int lines = 8;
                            string blanks = BlankLine(2);
                            int bytes = 17;
                            if (!TSPlayerB.isMobile[player.Index])
                            {
                                lines = 20;
                                blanks = BlankLine(60);
                                bytes = 0;
                            }
                            string message = $"{BlankLine(lines)}AutoPing: {ping}ms{blanks}";
                            if (TSPlayerB.PingStatus[player.Index])
                                player.SendData(PacketTypes.Status, message, bytes);
                            if (TSPlayerB.PingChat[player.Index])
                                player.SendInfoMessage($"AutoPing {ping}ms");
                            p.Dispose();
                        }
                    });
                } catch (Exception ex)
                {
                    TShock.Log.ConsoleError(ex.ToString());
                }
                //}, TaskScheduler.FromCurrentSynchronizationContext());
                await Task.Delay(10000);
            }
        }
    }
}
