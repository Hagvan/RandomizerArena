using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomizerArena
{
    /*public class Rounds : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "rounds";

        public string Help => "List all existing rounds.";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rounds" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (Randomizer.weapon_kits.Count == 0)
            {
                return;
            }
            string current = Randomizer.sets[0].name;
            for (int i = 1; i < Randomizer.sets.Count; i++)
            {
                current += ", " + Randomizer.sets[i].name;
                if (i % 3 == 2)
                {
                    UnturnedChat.Say(caller, current);
                    current = "";
                }
            }
            if (current.Length > 0)
            {
                UnturnedChat.Say(caller, current);
            }
        }
    }*/
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

    /*public class Test : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Console;

        public string Name => "test";

        public string Help => "Test!";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "test" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Asset[] items = Assets.find(EAssetType.ITEM);
            List<ItemGunAsset> guns = new List<ItemGunAsset>();
            List<ItemMagazineAsset> magazines = new List<ItemMagazineAsset>();
            List<ItemHatAsset> hats = new List<ItemHatAsset>();
            List<ItemShirtAsset> shirts = new List<ItemShirtAsset>();
            List<ItemVestAsset> vests = new List<ItemVestAsset>();
            List<ItemPantsAsset> pants = new List<ItemPantsAsset>();
            foreach (ItemAsset item in items)
            {
                if (item is ItemAsset)
                {
                    if (item.type == EItemType.GUN)
                    {
                        guns.Add((ItemGunAsset)item);
                    }
                    if (item.type == EItemType.MAGAZINE)
                    {
                        magazines.Add((ItemMagazineAsset)item);
                    }
                    if (item.type == EItemType.HAT)
                    {
                        if (((ItemClothingAsset) item).armor < 1 || ((ItemHatAsset)item).armor < 1)
                            hats.Add((ItemHatAsset)item);
                    }
                    if (item.type == EItemType.SHIRT)
                    {
                        if (((ItemClothingAsset)item).armor < 1 || ((ItemHatAsset)item).armor < 1)
                            shirts.Add((ItemShirtAsset)item);
                    }
                    if (item.type == EItemType.VEST)
                    {
                        //if (((ItemClothingAsset)item).armor < 1 || ((ItemHatAsset)item).armor < 1)
                        vests.Add((ItemVestAsset)item);
                    }
                    if (item.type == EItemType.PANTS)
                    {
                        if (((ItemClothingAsset)item).armor < 1 || ((ItemHatAsset)item).armor < 1)
                            pants.Add((ItemPantsAsset)item);
                    }
                }
            }
            List<WeaponKit> kits = new List<WeaponKit>();
            foreach (ItemGunAsset gun in guns)
            {
                if (gun.size_z == 0)
                {
                    continue;
                }
                WeaponKit kit = new WeaponKit
                {
                    weapon_id = gun.id,
                    magazines = new List<Magazine>()
                };
                foreach (ushort g_caliber in gun.magazineCalibers)
                {
                    foreach (ItemMagazineAsset magazine in magazines)
                    {
                        foreach (ushort m_caliber in magazine.calibers)
                        {
                            if (g_caliber == m_caliber)
                            {
                                Magazine m = new Magazine
                                {
                                    magazine_id = magazine.id,
                                    count = 3
                                };
                                kit.magazines.Add(m);
                            }
                        }
                    }
                }
                kits.Add(kit);
            }
            foreach (WeaponKit kit in kits)
            {
                string mags = "";
                foreach (Magazine magazine in kit.magazines)
                {
                    mags += magazine.magazine_id + ", ";
                }
                Console.WriteLine(kit.weapon_id + " - " + mags);
            }
            foreach (ItemShirtAsset shirt in shirts)
            {
                if (shirt.armor < 1 || shirt.explosionArmor < 1)
                {

                }
            }
        }
    }*/

    /*public class ChooseRound : IRocketCommand
    {
        private static WebClient web = new WebClient();

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "round";

        public string Help => "Next round will be guaranteed to be what you desire.";

        public string Syntax => "<round name>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "round" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0 || command[0].Length == 0)
            {
                UnturnedChat.Say(caller, "Usage: /round <round name>. Example: /round Ace - Ace only.");
                return;
            }

            UnturnedPlayer user = UnturnedPlayer.FromName(caller.DisplayName);

            if (user.IsAdmin)
            {
                int set_index = -1;
                if ((set_index = Randomizer.sets.FindIndex(set => set.name.ToLower().StartsWith(command[0].ToLower()))) > -1)
                {
                    if (Randomizer.selected_index.Contains(set_index))
                    {
                        UnturnedChat.Say(caller, "This round is already prioritised.");
                        return;
                    }
                    Randomizer.selected_index.Enqueue(set_index);
                    UnturnedChat.Say(UnturnedPlayer.FromName(caller.DisplayName).Player.name + " prioritised " +
                    Randomizer.sets[set_index].name + " round.");
                    UnturnedChat.Say("Type /round in the chat to prioritise your favorite round too!");
                }
                else
                {
                    UnturnedChat.Say(caller, "Round name not found. Try again. To view all rounds -> /rounds");
                }
                return;
            }

            //UnturnedChat.Say(caller, .ToString());
            string s = "";
            try
            {
                s = web.DownloadString("http://unturned-servers.net/api/?object=votes&element=claim&key=<your-key-here>&steamid="
                + user.SteamPlayer().playerID.steamID);
            }
            catch (WebException e)
            {
                Logger.LogWarning(e.ToString());
            }

            if (s.Length == 0)
            {
                return;
            }

            switch (s[0])
            {
                case '0':
                    UnturnedPlayer.FromName(caller.DisplayName).Player.sendBrowserRequest("You need to vote to use that.", "https://unturned-servers.net/server/196497/vote/");
                    UnturnedChat.Say(caller, "You didn't vote today. Please, vote to use this function.");
                    break;
                case '1':
                    int set_index = -1;
                    if ((set_index = Randomizer.sets.FindIndex(set => set.name.ToLower().StartsWith(command[0].ToLower()))) > -1)
                    {
                        if (Randomizer.selected_index.Contains(set_index))
                        {
                            UnturnedChat.Say(caller, "This round is already prioritised.");
                            return;
                        }
                        Randomizer.selected_index.Enqueue(set_index);
                        UnturnedChat.Say(UnturnedPlayer.FromName(caller.DisplayName).Player.name + " prioritised " +
                        Randomizer.sets[set_index].name + " round.");
                        UnturnedChat.Say("Type /round in the chat to prioritise your favorite round too!");
                        try
                        {
                            web.DownloadString("http://unturned-servers.net/api/?action=post&object=votes&element=claim&key=your-key-here&steamid="
                            + user.SteamPlayer().playerID.steamID);
                        }
                        catch (WebException e)
                        {
                            Logger.LogWarning(e.ToString());
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Round name not found. Try again.");
                    }
                    break;
                case '2':
                    UnturnedChat.Say(caller, "Sorry, but you used your round today.");
                    UnturnedChat.Say(caller, "In the future there will be more than one usage per vote!");
                    break;
            }
        }
    }
    */
}
