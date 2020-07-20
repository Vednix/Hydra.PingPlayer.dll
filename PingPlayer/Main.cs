using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Hydra.Extensions;
using Hydra.PingPlayer.Extensions;

namespace PingPlayer
{
    [ApiVersion(2, 1)]
    public class PlayerPing : TerrariaPlugin
    {
        public override Version Version => new Version(1, 0, 1, 3);

        public override string Name
        {
            get { return "Hydra.PingPlayer"; }
        }

        public override string Author
        {
            get { return "Vednix"; }
        }

        public override string Description
        {
            get { return "A plugin to send their player pings to them"; }
        }

        public PlayerPing(Main game) : base(game)
        {
            Order = 1;
        }
        internal static bool Wait = false;
        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(PingCmd, "ping"));
            ServerApi.Hooks.GamePostInitialize.Register(this, PostInitialize);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
        }
        public static bool[] internalPlayer;
        private static void PostInitialize(EventArgs args)
        {
            AutoPing.Run();
        }
        private static void OnJoin(JoinEventArgs args)
        {
            Wait = true;
            InternalTSPlayer.ResetPingStats(args.Who);
        }
        private static void OnGreet(GreetPlayerEventArgs args)
        {
            Wait = false;
        }
        private static void PingCmd(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                switch (args.Parameters[0])
                {
                    case "togglestatus":
                    case "status":
                        InternalTSPlayer.PingStatus[args.Player.Index] = !InternalTSPlayer.PingStatus[args.Player.Index];
                        InternalTSPlayer.PingChat[args.Player.Index] = false;
                            TSPlayerB.SendSuccessMessage(args.Player.Index, DefaultMessage: string.Format("Now you {0} receive ping via Status", InternalTSPlayer.PingStatus[args.Player.Index] ? "[c/98C807:will]" : "[c/ffa500:will not]"),
                                                                            PortugueseMessage: string.Format("Agora você {0} receber ping através de Status", InternalTSPlayer.PingStatus[args.Player.Index] ? "[c/98C807:irá]" : "[c/ffa500:não irá]"),
                                                                            SpanishMessage: string.Format("Ahora tu {0} ping a través del Estado", InternalTSPlayer.PingStatus[args.Player.Index] ? "[c/98C807:recibirá]" : "[c/ffa500:no hará]"));
                        break;
                    case "togglechat":
                    case "chat":
                        InternalTSPlayer.PingChat[args.Player.Index] = !InternalTSPlayer.PingChat[args.Player.Index];
                        InternalTSPlayer.PingStatus[args.Player.Index] = false;
                        TSPlayerB.SendSuccessMessage(args.Player.Index, DefaultMessage: string.Format("Now you {0} receive ping via Chat", InternalTSPlayer.PingChat[args.Player.Index] ? "[c/98C807:will]" : "[c/ffa500:will not]"),
                                                                        PortugueseMessage: string.Format("Agora você {0} receber ping através do Chat", InternalTSPlayer.PingChat[args.Player.Index] ? "[c/98C807:irá]" : "[c/ffa500:não irá]"),
                                                                        SpanishMessage: string.Format("Ahora tu {0} ping a través del Chat", InternalTSPlayer.PingChat[args.Player.Index] ? "[c/98C807:recibirá]" : "[c/ffa500:no hará]"));
                        break;
                    case "me":
                    case "eu":
                    case "yo":
                        Ping p = new Ping();
                        string ping = p.Send(args.Player.IP).RoundtripTime.ToString();
                        if (args.Player.IP == "127.0.0.1" || args.Player.IP.StartsWith("10.0.") || args.Player.IP.StartsWith("192.168.") && ping == "0")
                            ping = "<1";
                        if (ping != "0")
                            args.Player.SendInfoMessage($"Ping {ping}ms");
                        else
                        {
                            TSPlayerB.SendErrorMessage(args.Player.Index, DefaultMessage: "Your ISP does not allow the server to measure your ping",
                                                                          PortugueseMessage: "Seu provedor de internet não permite o servidor medir seu ping",
                                                                          SpanishMessage: "Su ISP no permite que el servidor mida su ping");
                        }
                        break;
                    default:
                        ShowCmds(args);
                        break;
                }
            }
            else
                ShowCmds(args);
        }
        private static void ShowCmds(CommandArgs args)
        {
            TSPlayerB.SendMessage(args.Player.Index, DefaultMessage: "PingPlayer - Commands", Color.Magenta,
                                                     PortugueseMessage: "PingPlayer - Comandos",
                                                     SpanishMessage: "PingPlayer - Comandos");

            TSPlayerB.SendMessage(args.Player.Index, DefaultMessage: "[c/ffd700:/ping togglestatus] => Enables or disables automatic pinging via Status", Color.LightGray,
                                                     PortugueseMessage: "[c/ffd700:/ping status] => Ativa ou desativa o envio automatico de pings através de Status",
                                                     SpanishMessage: "[c/ffd700:/ping status] => Habilita o deshabilita el ping automático a través del estado");

            TSPlayerB.SendMessage(args.Player.Index, DefaultMessage: "[c/ffd700:/ping togglechat] => Enables or disables automatic pinging via Chat", Color.LightGray,
                                                     PortugueseMessage: "[c/ffd700:/ping chat] => Ativa ou desativa o envio automatico de pings através do Chat",
                                                     SpanishMessage: "[c/ffd700:/ping chat] => Activa o desactiva el ping automático a través del chat");

            TSPlayerB.SendMessage(args.Player.Index, DefaultMessage: "[c/ffd700:/ping me] => See your ping only once", Color.LightGray,
                                                     PortugueseMessage: "[c/ffd700:/ping me] => Ver seu ping uma única vez apenas",
                                                     SpanishMessage: "[c/ffd700:/ping yo] => Ver tu ping solo una vez");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Commands.ChatCommands.Remove(new Command(PingCmd, "ping"));
                ServerApi.Hooks.GamePostInitialize.Deregister(this, PostInitialize);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreet);
            }
            base.Dispose(disposing);
        }
    }
}
