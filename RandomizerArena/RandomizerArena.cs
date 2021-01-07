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
    public class Randomizer : RocketPlugin<RandomizerConfiguration>
    {
        private Random r;
        private int round_counter;
        private List<WeaponKit> weapon_kits;
        private List<Pants> pants;
        private List<Hat> hats;
        private List<Shirt> shirts;
        private List<Melee> melees;
        private int protection_duration;
        private uint start_experience;
        private int current_duration;
        private bool maxskills;

        protected override void Load()
        {
            base.Load();
            round_counter = 0;
            protection_duration = 10;
            start_experience = 150;
            maxskills = false;

            weapon_kits = Configuration.Instance.weapon_kits;
            pants = Configuration.Instance.a_pants;
            hats = Configuration.Instance.a_hat;
            shirts = Configuration.Instance.a_shirt;
            melees = Configuration.Instance.a_melees;
            protection_duration = Configuration.Instance.protection_duration;
            start_experience = Configuration.Instance.start_experience;
            maxskills = Configuration.Instance.maxskills;

            current_duration = -1;
            r = new Random();
            StartCoroutine(CheckArenaState());
            Logger.Log("Loaded arena randomizer.");
        }
        protected override void Unload()
        {
            base.Unload();
            StopAllCoroutines();
            Logger.Log("Unloaded arena randomizer.");
        }
        EArenaState state;
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
                            case EArenaState.LOBBY:
                                {
                                    //Logger.Log("Arena state: LOBBY");
                                    break;
                                }
                            case EArenaState.CLEAR:
                                {
                                    //Logger.Log("Arena state: CLEAR");
                                    break;
                                }
                            case EArenaState.WARMUP:
                                {
                                    //Logger.Log("Arena state: WARMUP");
                                    break;
                                }
                            case EArenaState.SPAWN:
                                {
                                    //Logger.Log("Arena state: SPAWN");
                                    break;
                                }
                            case EArenaState.PLAY:
                                {
                                    //Logger.Log("Arena state: PLAY");
                                    ItemManager.askClearAllItems(); // remove all items from the ground

                                    RandomizerSpawnProtection.StartProtection(); // protect players
                                    current_duration = protection_duration;

                                    // todo equip player
                                    WeaponKit round_weaponkit = weapon_kits[r.Next(weapon_kits.Count)]; // randomly select loadout items
                                    Magazine round_magazine = round_weaponkit.magazines[r.Next(round_weaponkit.magazines.Count)];
                                    Hat round_hat = hats[r.Next(hats.Count)];
                                    Shirt round_shirt = shirts[r.Next(hats.Count)];
                                    Pants round_pants = pants[r.Next(hats.Count)];
                                    Melee round_melee = melees[r.Next(melees.Count)];

                                    for (int i = 0; i < Provider.clients.Count; i++) // give everybody the loadout
                                    {
                                        UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(Provider.clients[i].playerID.steamID);
                                        EquipPlayer(uPlayer, round_weaponkit, round_magazine, round_hat, round_shirt, round_pants, round_melee);
                                    }

                                    break;
                                }
                            case EArenaState.FINALE:
                                {
                                    //Logger.Log("Arena state: FINALE");
                                    break;
                                }
                            case EArenaState.RESTART:
                                {
                                    //Logger.Log("Arena state: RESTART");
                                    break;
                                }
                            case EArenaState.INTERMISSION:
                                {
                                    if (round_counter == 2)
                                    {
                                        UnturnedChat.Say("Enjoying the gamemode? Want to make a suggestion or apply for moderator? Join the server discord! Say /discord");
                                        round_counter = 0;
                                    }
                                    else
                                    {
                                        round_counter++;
                                    }
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
            if (maxskills)
            {
                player.MaxSkills();
            } 
            else
            {
                player.Experience = start_experience;
            }
        }

        public readonly byte[] EMPTY_BYTE_ARRAY = new byte[0]; // used only in ClearInventory from uEssentials, modified for RandomizerArena

        private void ClearInventory(UnturnedPlayer player)
        {
            var playerInv = player.Inventory;


            // "Remove "models" of items from player "body""
            player.Player.channel.send("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER,
                (byte)0, (byte)0, EMPTY_BYTE_ARRAY);
            player.Player.channel.send("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER,
                (byte)1, (byte)0, EMPTY_BYTE_ARRAY);

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
            System.Action removeUnequipped = () => {
                for (byte i = 0; i < playerInv.getItemCount(2); i++)
                {
                    playerInv.removeItem(2, 0);
                }
            };


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