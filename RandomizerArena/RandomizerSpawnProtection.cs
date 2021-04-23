using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace RandomizerArena
{
    class RandomizerSpawnProtection
    {
        public static void StartProtection()
        {
            foreach (SteamPlayer player in Provider.clients)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(player);
                uPlayer.GodMode = true;
                uPlayer.VanishMode = true;
            }
            Logger.Log("Player protection started!");
            UnturnedChat.Say("Protection started! Choose your skills and position!");
        }

        public static void RemoveProtection()
        {
            foreach (SteamPlayer player in Provider.clients)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(player);
                uPlayer.GodMode = false;
                uPlayer.VanishMode = false;
            }
            Logger.Log("Player protection ended!");
            UnturnedChat.Say("Time is up! Protection ended!");
        }
    }
}
