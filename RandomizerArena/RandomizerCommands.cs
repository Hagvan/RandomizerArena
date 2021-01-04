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
            Asset[] items = Assets.find(EAssetType.ITEM);
            List<ItemGunAsset> guns = new List<ItemGunAsset>();
            List<ItemMagazineAsset> magazines = new List<ItemMagazineAsset>();
            List<ItemHatAsset> hats = new List<ItemHatAsset>();
            List<ItemShirtAsset> shirts = new List<ItemShirtAsset>();
            List<ItemVestAsset> vests = new List<ItemVestAsset>();
            List<ItemPantsAsset> pants = new List<ItemPantsAsset>();
            List<ItemMeleeAsset> melees = new List<ItemMeleeAsset>();
            foreach (ItemAsset item in items)
            {
                if (item is ItemAsset)
                {
                    switch (item.type)
                    {
                        case EItemType.GUN:
                            guns.Add((ItemGunAsset)item);
                            break;
                        case EItemType.MAGAZINE:
                            magazines.Add((ItemMagazineAsset)item);
                            break;
                        case EItemType.HAT:
                            if (((ItemClothingAsset)item).armor < 1 || ((ItemClothingAsset)item).armor < 1)
                                hats.Add((ItemHatAsset)item);
                            break;
                        case EItemType.SHIRT:
                            if (((ItemClothingAsset)item).armor < 1 || ((ItemClothingAsset)item).armor < 1)
                                shirts.Add((ItemShirtAsset)item);
                            break;
                        case EItemType.VEST:
                            //if (((ItemClothingAsset)item).armor < 1 || ((ItemHatAsset)item).armor < 1)
                            vests.Add((ItemVestAsset)item);
                            break;
                        case EItemType.PANTS:
                            if (((ItemClothingAsset)item).armor < 1 || ((ItemClothingAsset)item).armor < 1)
                                pants.Add((ItemPantsAsset)item);
                            break;
                        case EItemType.MELEE:
                            melees.Add((ItemMeleeAsset)item);
                            break;
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
            /*foreach (ItemShirtAsset shirt in shirts)
            {
                if (shirt.armor < 1 || shirt.explosionArmor < 1)
                {

                }
            }*/
            foreach (ItemMeleeAsset melee in melees)
            {
                if (melee.size_z == 0)
                {
                    continue;
                }
                Console.WriteLine(melee.id + ": " + melee.size_z + ", " + melee.size2_z + ", " + melee.size_x + ", " + melee.size_y);
            }
        }
    }
}
