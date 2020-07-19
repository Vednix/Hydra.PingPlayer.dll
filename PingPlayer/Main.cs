using Microsoft.Xna.Framework;
using PingPlayer.Extensions;
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

namespace PingPlayer
{
    [ApiVersion(2, 1)]
    public class PlayerPing : TerrariaPlugin
    {
        public override Version Version => new Version(1, 0, 0, 0);

        public override string Name
        {
            get { return "PingPlayer"; }
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
            TSPlayerExt.Reset(args.Who);
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
                        TSPlayerExt.PingStatus[args.Player.Index] = !TSPlayerExt.PingStatus[args.Player.Index];
                        TSPlayerExt.PingChat[args.Player.Index] = false;
                        if (args.Player.IsPortuguese)
                            args.Player.SendSuccessMessage(string.Format("Agora você {0} receber ping através de Status", TSPlayerExt.PingStatus[args.Player.Index] ? "[c/98C807:irá]" : "[c/ffa500:não irá]"));
                        else
                            args.Player.SendSuccessMessage(string.Format("Now you {0} receive ping via Status", TSPlayerExt.PingStatus[args.Player.Index] ? "[c/98C807:will]" : "[c/ffa500:will not]"));
                        break;
                    case "togglechat":
                    case "chat":
                        TSPlayerExt.PingChat[args.Player.Index] = !TSPlayerExt.PingChat[args.Player.Index];
                        TSPlayerExt.PingStatus[args.Player.Index] = false;
                        if (args.Player.IsPortuguese)
                            args.Player.SendSuccessMessage(string.Format("Agora você {0} receber ping através do Chat", TSPlayerExt.PingChat[args.Player.Index] ? "[c/98C807:irá]" : "[c/ffa500:não irá]"));
                        else
                            args.Player.SendSuccessMessage(string.Format("Now you {0} receive ping via Chat", TSPlayerExt.PingChat[args.Player.Index] ? "[c/98C807:will]" : "[c/ffa500:will not]"));
                        break;
                    case "me":
                        Ping p = new Ping();
                        string ping = p.Send(args.Player.IP).RoundtripTime.ToString();
                        if (int.Parse(ping) != 0)
                            args.Player.SendInfoMessage($"Ping {ping}ms");
                        else
                        {
                            if (args.Player.IsPortuguese)
                                args.Player.SendErrorMessage("Seu provedor de internet não permite o servidor medir seu ping");
                            else
                                args.Player.SendErrorMessage("Your ISP does not allow the server to measure your ping");
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
            if (args.Player.IsPortuguese)
            {
                args.Player.SendMessage("PingPlayer - Comandos", Color.Magenta);
                args.Player.SendMessage("[c/ffd700:/ping status] => Ativa ou desativa o envio automatico de pings através de Status", Color.LightGray);
                args.Player.SendMessage("[c/ffd700:/ping chat] => Ativa ou desativa o envio automatico de pings através do Chat", Color.LightGray);
                args.Player.SendMessage("[c/ffd700:/ping me] => Ver seu ping uma única vez apenas", Color.LightGray);
            }
            else
            {
                args.Player.SendMessage("PingPlayer - Commands", Color.Magenta);
                args.Player.SendMessage("[c/ffd700:/ping togglestatus] => Enables or disables automatic pinging via Status", Color.LightGray);
                args.Player.SendMessage("[c/ffd700:/ping togglechat] => Enables or disables automatic pinging via Chat", Color.LightGray);
                args.Player.SendMessage("[c/ffd700:/ping me] => See your ping only once", Color.LightGray);
            }
        }
    }
}
