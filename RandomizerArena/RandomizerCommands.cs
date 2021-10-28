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

    public class Balance : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "balance";

        public string Help => "Check your balance.";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "balance" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = UnturnedPlayer.FromName(caller.DisplayName);
            UnturnedChat.Say(player, "Your balance is " + RandomizerArena.economy.GetBalance(player) + ".");
        }
    }

    public class BuyRound : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "buyround";

        public string Help => "Use your balance to queue a specific weapon for next round.";

        public string Syntax => "/buyround weapon_id";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "buyround" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            //UnturnedPlayer player = UnturnedPlayer.FromName(caller.DisplayName);
            //UnturnedChat.Say(player, "Your balance is " + RandomizerArena.economy.GetBalance(player) + ".");
        }
    }

    public class RandomizerAdReward : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Console;

        public string Name => "randomizeradreward";

        public string Help => "";

        public string Syntax => "randomizeradreward steam_id";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "randomizeradreward" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            ulong steam_id;
            if (command.Length > 0)
            {
                if (ulong.TryParse(command[0], out steam_id))
                {
                    RandomizerArena.economy.RewardAd(steam_id);
                }
                else
                {
                    UnturnedLog.warn("There was an issue with giving ad reward to " + command[0]);
                }
            }
            else
            {
                UnturnedLog.warn("There was an issue with giving ad reward due to command.length = 0");
            }

        }
    }

    public class Nominate : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "Nominate";

        public string Help => "Nominates a weapon to be used by everyone in the next round. Costs 1500 credits.";

        public string Syntax => "<gun id>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "nominate" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = UnturnedPlayer.FromName(caller.DisplayName);
            if (command.Length > 0)
            {
                if (ushort.TryParse(command[0], out ushort item_id))
                {
                    WeaponKit kit = RandomizerArena.weapon_kits.Find(x => x.weapon_id == item_id);
                    if (kit != null)
                    {
                        if (RandomizerArena.economy.PayForNominate(player.CSteamID.m_SteamID))
                        {
                            RandomizerArena.nominate_queue.Enqueue(kit);
                            UnturnedChat.Say(player, "Successfully nominated " + kit.weapon_name + "! You can enqueue multiple weapons if you like!");
                        }
                        else
                        {
                            // not enough balance
                            UnturnedChat.Say(player, "You don't have enough balance to use this command. Earn credits by fragging in the arena and watching ads.");
                        }
                    }
                    else
                    {
                        // invalid weapon id
                        UnturnedChat.Say(player, "Invalid gun id. For (mostly) all weapon ids you can use with this command, say /weaponids");
                    }
                }
                else
                {
                    // invalid argument
                    UnturnedChat.Say(player, "Invalid argument. Example for this command: /nominate 18 - will nominate timberwolfs in the queue.");
                }
            } 
            else
            {
                UnturnedChat.Say(player, "Usage: /nominate <gun id>. For example: /nominate 18 - nominate timberwolf.");
            }
        }
    }

    public class WeaponIDs : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "weaponids";

        public string Help => "Opens a browser tab with all weapon ids.";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "weaponids" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer.FromName(caller.DisplayName).Player.sendBrowserRequest("Opens a tab in the browser to look for weapon ids.", "https://unturneditems.com/weapons");
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
