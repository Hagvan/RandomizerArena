using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomizerArena
{
    public class Discord : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "discord";

        public string Help => "Join our discord!";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "discord" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer.FromName(caller.DisplayName).Player.sendBrowserRequest("Come join our discord!", "https://discord.gg/5NsRr8aQu2");
        }
    }

    public class Donate : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "donate";

        public string Help => "Donate to support the server!";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "donate" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer.FromName(caller.DisplayName).Player.sendBrowserRequest("Please consider donating a dollar or two to support the server!", "https://eu-randomizer-arena.tebex.io/category/1811468");
        }
    }

    public class Test : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Console;

        public string Name => "test";

        public string Help => "Test!";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "test" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            RandomizerArena.random_assets.InitLists(true);
        }
    }
}
