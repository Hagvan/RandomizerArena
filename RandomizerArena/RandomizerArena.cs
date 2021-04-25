using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using Random = System.Random;

namespace RandomizerArena
{
    public class RandomizerArena : RocketPlugin<RandomizerConfiguration>
    {
        public static readonly RandomizerAssets random_assets = new RandomizerAssets();
        private readonly Random random = new Random();
        private int round_counter = 0;
        private int protection_duration = 10;
        private int current_duration = -1;
        private bool maxskills = false;
        private uint start_experience = 150;
        private List<WeaponKit> weapon_kits;
        private List<Pants> pants;
        private List<Hat> hats;
        private List<Shirt> shirts;
        private List<Vest> vests;
        private List<Melee> melees;

        protected override void Load()
        {
            random_assets.InitLists(false);
            base.Load();
            weapon_kits = random_assets.weapon_kits;
            pants = random_assets.a_pants;
            hats = random_assets.a_hat;
            shirts = random_assets.a_shirt;
            vests = random_assets.a_vest;
            melees = random_assets.a_melees;
            protection_duration = Configuration.Instance.protection_duration;
            start_experience = Configuration.Instance.start_experience;
            maxskills = Configuration.Instance.maxskills;
            StartCoroutine(CheckArenaState());
            Logger.Log("Loaded arena randomizer.");
        }
        protected override void Unload()
        {
            base.Unload();
            StopAllCoroutines();
            Logger.Log("Unloaded arena randomizer.");
        }

        private EArenaState state;
        private IEnumerator CheckArenaState()
        {
            while (true)
            {
                if (Provider.isServer)
                {
                    if (current_duration > 0)
                    {
                        current_duration--;
                    }
                    else if (current_duration == 0)
                    {
                        RandomizerSpawnProtection.RemoveProtection();
                        current_duration--;
                    }

                    if (state != LevelManager.arenaState) {
                        state = LevelManager.arenaState;
                        switch (state)
                        {
                            case EArenaState.PLAY:
                                {
                                    ItemManager.askClearAllItems(); // remove all items from the ground

                                    RandomizerSpawnProtection.StartProtection(); // protect players
                                    current_duration = protection_duration;

                                    Logger.Log($"{weapon_kits.Count}, {hats.Count}, {pants.Count}, {vests.Count}, {melees.Count}");
                                    WeaponKit round_weaponkit = weapon_kits[random.Next(weapon_kits.Count)]; // randomly select loadout items
                                    Magazine round_magazine = round_weaponkit.magazines[random.Next(round_weaponkit.magazines.Count)];
                                    Hat round_hat = hats[random.Next(hats.Count)];
                                    Shirt round_shirt = shirts[random.Next(hats.Count)];
                                    Pants round_pants = pants[random.Next(hats.Count)];
                                    //Vest round_vest = vests[random.Next(vests.Count)];
                                    Melee round_melee = melees[random.Next(melees.Count)];

                                    foreach (SteamPlayer player in Provider.clients) // give everybody the loadout
                                    {
                                        UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(player);
                                        //EquipPlayer(uPlayer, round_weaponkit, round_magazine, round_hat, round_shirt, round_pants, round_vest, round_melee);
                                        EquipPlayer(uPlayer, round_weaponkit, round_magazine, round_hat, round_shirt, round_pants, round_melee);
                                    }
                                    break;
                                }
                            case EArenaState.INTERMISSION:
                                {
                                    switch (round_counter)
                                    {
                                        case 0:
                                            // advertise supporter perks and command to purchase
                                        case 1:
                                            // advertise survival server
                                            break;
                                        case 2:
                                            UnturnedChat.Say("Enjoying the gamemode? Want to make a suggestion or apply for moderator? Join the server discord! Say /discord");
                                            round_counter = 0;
                                            break;
                                    }
                                    round_counter++;
                                    break;
                                }
                        }
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }
        private void EquipPlayer(UnturnedPlayer player, WeaponKit weaponKit, Magazine magazine, Hat hat, Shirt shirt, Pants pants, Melee melee)
        {
            ClearInventory(player); // forcefully remove inventory to prevent players from keeping the kit from previous round by not respawning
            player.GiveItem(weaponKit.weapon_id, 1);
            player.GiveItem(394, 3); // give 3 dressings by default
            player.GiveItem(magazine.magazine_id, magazine.count);
            player.GiveItem(hat.hat_id, 1);
            player.GiveItem(shirt.shirt_id, 1);
            player.GiveItem(pants.pants_id, 1);
            player.GiveItem(melee.melee_id, 1);
            //player.GiveItem(vest.vest_id, 1);
            if (maxskills)
            {
                //player.MaxSkills();
            } 
            else
            {
                player.Experience = start_experience;
            }
        }

        public readonly byte[] EMPTY_BYTE_ARRAY = new byte[0]; // used only in ClearInventory from uEssentials, modified for RandomizerArena

        [System.Obsolete]
        private void ClearInventory(UnturnedPlayer player)
        {
            var playerInv = player.Inventory;
            // "Remove "models" of items from player "body""
            player.Player.channel.send("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, (byte)0, (byte)0, EMPTY_BYTE_ARRAY);
            player.Player.channel.send("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, (byte)1, (byte)0, EMPTY_BYTE_ARRAY);
            // Remove items
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                if (page == PlayerInventory.AREA)
                    continue;

                var count = playerInv.getItemCount(page);

                for (byte index = 0; index < count; index++)
                {
                    playerInv.removeItem(page, 0);
                }
            }
            // Remove clothes
            // Remove unequipped cloths
            void removeUnequipped()
            {
                for (byte i = 0; i < playerInv.getItemCount(2); i++)
                {
                    playerInv.removeItem(2, 0);
                }
            }
            // Unequip & remove from inventory
            player.Player.clothing.askWearBackpack(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.Player.clothing.askWearGlasses(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.Player.clothing.askWearHat(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.Player.clothing.askWearPants(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.Player.clothing.askWearMask(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.Player.clothing.askWearShirt(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.Player.clothing.askWearVest(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();
        }

    }
}