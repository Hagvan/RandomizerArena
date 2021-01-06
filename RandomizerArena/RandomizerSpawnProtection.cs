using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RandomizerArena
{
    class RandomizerSpawnProtection
    {
        public static void StartProtection()
        {
            foreach (SteamPlayer player in Provider.clients)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(player.playerID.steamID);
                uPlayer.Features.GodMode = true;
                uPlayer.Features.VanishMode = true;
            }
            Logger.Log("Player protection started!");
            UnturnedChat.Say("Protection started! Choose your skills and position!");
        }

        public static void RemoveProtection()
        {
            foreach (SteamPlayer player in Provider.clients)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(player.playerID.steamID);
                uPlayer.Features.GodMode = false;
                uPlayer.Features.VanishMode = false;
            }
            Logger.Log("Player protection ended!");
            UnturnedChat.Say("Time is up! Protection ended!");
        }
    }
}
