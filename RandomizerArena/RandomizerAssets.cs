using Rocket.API;
using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace RandomizerArena
{
    public class RandomizerAssets
    {
        public List<WeaponKit> weapon_kits;
        public List<Shirt> a_shirt;
        public List<Vest> a_vest;
        public List<Pants> a_pants;
        public List<Hat> a_hat;
        public List<Melee> a_melees;

        public RandomizerAssets()
        {
            weapon_kits = new List<WeaponKit>();
            a_shirt = new List<Shirt>();
            a_vest = new List<Vest>();
            a_pants = new List<Pants>();
            a_hat = new List<Hat>();
            a_melees = new List<Melee>();
        }

        public void InitLists(bool test)
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
                if (item.canPlayerEquip)
                {
                    switch (item.type)
                    {
                        case EItemType.GUN:
                            if (item.size_x > 1 && item.size_y > 1) // all proper guns in the game have size over 1x1
                            {
                                guns.Add((ItemGunAsset)item);
                            }
                            break;
                        case EItemType.MAGAZINE:
                            magazines.Add((ItemMagazineAsset)item);
                            break;
                        case EItemType.SIGHT:
                            if (!item.name.Contains("Iron_Sights")) // I don't want iron sights in the sights pool
                            {
                                sights.Add((ItemSightAsset)item);
                            }
                            break;
                        case EItemType.HAT:
                            //if (((ItemClothingAsset)item).armor < 1 || ((ItemClothingAsset)item).explosionArmor < 1)
                            //{
                            hats.Add((ItemHatAsset)item);
                            //}
                            break;
                        case EItemType.SHIRT:
                            //if (((ItemClothingAsset)item).armor < 1 || ((ItemClothingAsset)item).explosionArmor < 1)
                            //{
                            shirts.Add((ItemShirtAsset)item);
                            //}
                            break;
                        case EItemType.VEST:
                            //if (((ItemClothingAsset)item).armor < 1 || ((ItemHatAsset)item).explosionArmor < 1)
                            //{
                            vests.Add((ItemVestAsset)item);
                            //}
                            break;
                        case EItemType.PANTS:
                            //if (((ItemClothingAsset)item).armor < 1 || ((ItemClothingAsset)item).explosionArmor < 1)
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
                WeaponKit kit = new WeaponKit
                {
                    weapon_id = gun.id,
                    magazines = new List<Magazine>(),
                    sights = new List<Sight>()
                };
                foreach (ushort g_caliber in gun.magazineCalibers) // get gun's magasine options
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
                foreach (ushort g_caliber in gun.attachmentCalibers) // get gun's attachment options
                {
                    //if (test) Console.WriteLine("1) g_caliber = " + g_caliber);
                    foreach (ItemSightAsset sight in sights)
                    {
                        //if (test) Console.WriteLine("   2) sight_id = " + sight.id);
                        foreach (ushort s_caliber in sight.calibers) // sight attachments
                        {
                            //if (test) Console.WriteLine("      3) sight_caliber = " + s_caliber);
                            if (g_caliber == s_caliber)
                            {
                                Sight s = new Sight
                                {
                                    sight_id = sight.id,
                                };
                                kit.sights.Add(s);
                                //if (test) Console.WriteLine("Added sight: " + sight.id);
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
                //Console.WriteLine(kit.weapon_id + " - " + mags + " / " + attachments);
            }
            a_shirt = new List<Shirt>();
            foreach (ItemShirtAsset shirt in shirts)
            {
                a_shirt.Add(new Shirt() { shirt_id = shirt.id });
                if (test) Console.WriteLine(shirt.name + "|" + shirt.id + "|" + shirt.type + "|" + shirt.canPlayerEquip + "|" + shirt.canUse + "|" + shirt.size_x + "|" + shirt.size_y);
            }
            a_vest = new List<Vest>();
            foreach (ItemVestAsset vest in vests)
            {
                a_vest.Add(new Vest() { vest_id = vest.id });
                if (test) Console.WriteLine(vest.name + "|" + vest.id + "|" + vest.type + "|" + vest.canPlayerEquip + "|" + vest.canUse + "|" + vest.size_x + "|" + vest.size_y);
            }
            a_hat = new List<Hat>();
            foreach (ItemHatAsset l_hat in hats)
            {
                a_hat.Add(new Hat() { hat_id = l_hat.id });
                if (test) Console.WriteLine(l_hat.name + "|" + l_hat.id + "|" + l_hat.type + "|" + l_hat.canPlayerEquip + "|" + l_hat.canUse + "|" + l_hat.size_x + "|" + l_hat.size_y);
            }
            a_pants = new List<Pants>();
            foreach (ItemPantsAsset l_pants in pants)
            {
                a_pants.Add(new Pants() { pants_id = l_pants.id });
                if (test) Console.WriteLine(l_pants.name + "|" + l_pants.id + "|" + l_pants.type + "|" + l_pants.canPlayerEquip + "|" + l_pants.canUse + "|" + l_pants.size_x + "|" + l_pants.size_y);
            }
            a_melees = new List<Melee>();
            foreach (ItemMeleeAsset melee in melees)
            {
                a_melees.Add(new Melee() { melee_id = melee.id });
                if (test) Console.WriteLine(melee.name + "|" + melee.id + "|" + melee.type + "|" + melee.canPlayerEquip + "|" + melee.canUse + "|" + melee.size_x + "|" + melee.size_y);
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

    // melee weapons
    public class Melee
    {
        public ushort melee_id;

        public Melee() { }
    }

    // clothes
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

    public class Vest
    {
        public ushort vest_id;

        public Vest() { }
    }

    public class Pants
    {
        public ushort pants_id;

        public Pants() { }
    }
}
