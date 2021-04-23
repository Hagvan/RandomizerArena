using Rocket.API;
using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace RandomizerArena
{
    public class RandomizerConfiguration : IRocketPluginConfiguration
    {
        public int protection_duration;
        public uint start_experience;
        public bool maxskills;
        public List<WeaponKit> weapon_kits;
        public List<Shirt> a_shirt;
        public List<Pants> a_pants;
        public List<Hat> a_hat;
        public List<Melee> a_melees;

        public void LoadDefaults()
        {
            protection_duration = 10; // set default protection duration
            start_experience = 150; // set default round start experience
            maxskills = false; // by default max skills are disabled
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
            weapon_kits = new List<WeaponKit>();
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
            a_shirt = new List<Shirt>();
            foreach (ItemShirtAsset shirt in shirts)
            {
                if (shirt.armor < 1 || shirt.explosionArmor < 1)
                {
                    a_shirt.Add(new Shirt() { shirt_id = shirt.id });
                }
            }
            a_hat = new List<Hat>();
            foreach (ItemHatAsset l_hat in hats)
            {
                if (l_hat.armor < 1 || l_hat.explosionArmor < 1)
                {
                    a_hat.Add(new Hat() { hat_id = l_hat.id });
                }
            }
            a_pants = new List<Pants>();
            foreach (ItemPantsAsset l_pants in pants)
            {
                if (l_pants.armor < 1 || l_pants.explosionArmor < 1)
                {
                    a_pants.Add(new Pants() { pants_id = l_pants.id });
                }
            }
            a_melees = new List<Melee>();
            foreach (ItemMeleeAsset l_melee in melees)
            {
                if (l_melee.size_x == 0)
                {
                    continue;
                }
                a_melees.Add(new Melee() { melee_id = l_melee.id });
            }
        }
    }
    // WeaponKit - data model to link between weapon and it's magazines (and their count)
    public class WeaponKit
    {
        public ushort weapon_id;
        public List<Magazine> magazines;
        public List<Sight> sights;

        public WeaponKit() { }

        public WeaponKit(List<Magazine> magazines, List<Sight> sights) { 
            foreach (Magazine magazine in magazines)
            {
                this.magazines.Add(magazine);
            }
            foreach (Sight sight in sights)
            {
                this.sights.Add(sight);
            }
        }
    }

    // Magazine - data model to store magazine type and how much of it to give
    public class Magazine
    {
        public ushort magazine_id;
        public byte count;

        public Magazine() { }

        public Magazine(ushort magazine_id, byte count) {
            this.magazine_id = magazine_id;
            this.count = count;
        }
    }
    public class Sight
    {
        public ushort sight_id;

        public Sight() { }

        public Sight(ushort sight_id)
        {
            this.sight_id = sight_id;
        }
    }

    public class Hat
    {
        public ushort hat_id;

        public Hat() { }
    }

    public class Shirt
    {
        public ushort shirt_id;

        public Shirt() { }
    }

    public class Pants
    {
        public ushort pants_id;

        public Pants() { }
    }

    public class Melee
    {
        public ushort melee_id;

        public Melee() { }
    }
}
