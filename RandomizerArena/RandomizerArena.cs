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

        protected override void Load()
        {
            base.Load();
            round_counter = 0;
            weapon_kits = Configuration.Instance.weapon_kits;
            pants = Configuration.Instance.a_pants;
            hats = Configuration.Instance.a_hat;
            shirts = Configuration.Instance.a_shirt;
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
                if (Provider.isServer && state != LevelManager.arenaState)
                {
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
                                ItemManager.askClearAllItems();

                                // todo equip player
                                WeaponKit round_weaponkit = weapon_kits[r.Next(weapon_kits.Count)];
                                Magazine round_magazine = round_weaponkit.magazines[r.Next(round_weaponkit.magazines.Count)];
                                Hat round_hat = hats[r.Next(hats.Count)];
                                Shirt round_shirt = shirts[r.Next(hats.Count)];
                                Pants round_pants = pants[r.Next(hats.Count)];

                                for (int i = 0; i < Provider.clients.Count; i++)
                                {
                                    UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(Provider.clients[i].playerID.steamID);
                                    EquipPlayer(uPlayer, round_weaponkit, round_magazine, round_hat, round_shirt, round_pants);
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
                                if (round_counter == 3)
                                {
                                    UnturnedChat.Say("Enjoying the gamemode? Want to suggest a change or a new round? Join our discord! /discord");
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
                yield return new WaitForSeconds(1f);
            }
        }
        private void EquipPlayer(UnturnedPlayer player, WeaponKit weaponKit, Magazine magazine, Hat hat, Shirt shirt, Pants pants)
        {
            player.GiveItem(weaponKit.weapon_id, 1);
            player.GiveItem(394, 3); // give 3 dressings by default
            player.GiveItem(magazine.magazine_id, magazine.count);
            player.GiveItem(hat.hat_id, 1);
            player.GiveItem(shirt.shirt_id, 1);
            player.GiveItem(pants.pants_id, 1);
            player.GiveItem(105, 1); // baseball bat as default melee for every round
            player.MaxSkills();
        }
    }
}