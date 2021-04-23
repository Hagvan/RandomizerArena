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
            Asset[] items = Assets.find(EAssetType.ITEM); // generate equipment data
            List<ItemGunAsset> guns = new List<ItemGunAsset>();
            List<ItemMagazineAsset> magazines = new List<ItemMagazineAsset>();
            List<ItemSightAsset> sights = new List<ItemSightAsset>();
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
                        case EItemType.SIGHT:
                            sights.Add((ItemSightAsset)item);
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
            List<WeaponKit> weapon_kits = new List<WeaponKit>();
            foreach (ItemGunAsset gun in guns) // add all weapons
            {
                if (gun.isTurret)
                {
                    //Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", gun.id, gun.isTurret, gun.isPro, gun.isInvulnerable, gun.shell, gun.hasSafety, gun.action, gun.canPlayerEquip);
                    continue; // avoid using vehicle weapons
                }
                WeaponKit kit = new WeaponKit
                {
                    weapon_id = gun.id,
                    magazines = new List<Magazine>(),
                    sights = new List<Sight>()
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
                foreach (ushort g_caliber in gun.attachmentCalibers)
                {
                    Console.WriteLine("1) g_caliber = " + g_caliber);
                    foreach (ItemSightAsset sight in sights)
                    {
                        Console.WriteLine("   2) sight_id = " + sight.id);
                        foreach (ushort s_caliber in sight.calibers)
                        {
                            Console.WriteLine("      3) sight_caliber = " + s_caliber);
                            if (g_caliber == s_caliber)
                            {
                                Sight s = new Sight
                                {
                                    sight_id = sight.id,
                                };
                                kit.sights.Add(s);
                                //Console.WriteLine("Added sight: " + sight.id);
                            }
                        }
                    }
                }
                weapon_kits.Add(kit);
            }
            foreach (WeaponKit kit in weapon_kits) // add all magazines for weapons
            {
                string mags = "";
                foreach (Magazine magazine in kit.magazines)
                {
                    mags += magazine.magazine_id + ", ";
                }
                string attachments = "";
                foreach (Sight attachment in kit.sights)
                {
                    attachments += attachment.sight_id + ", ";
                }
                Console.WriteLine(kit.weapon_id + " - " + mags + " / " + attachments);
            }
        }
    }
}
