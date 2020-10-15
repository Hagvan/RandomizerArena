using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using Random = System.Random;



namespace RandomizerArena
{
    public class Randomizer : RocketPlugin<RandomizerConfiguration>
    {
        private Random r;
        private RandomSet set;
        public static List<RandomSet> sets;
        private List<int> pool;
        private LinkedList<int> enqueued;
        private int round_counter;
        public static Queue<int> selected_index;

        protected override void Load()
        {
            base.Load();
            round_counter = 0;
            selected_index = new Queue<int>();
            sets = Configuration.Instance.random_sets;
            r = new Random();
            pool = new List<int>();
            for (int i = 0; i < sets.Count; i++)
            {
                pool.Add(i);
            }
            enqueued = new LinkedList<int>();
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

                                //Logger.Log("Selected index = " + selected_index);

                                int current;
                                if (selected_index.Count == 0) // no prioritized rounds
                                {
                                    current = pool[r.Next(0, pool.Count)];
                                }
                                else // play the prioritized round
                                {
                                    enqueued.Remove(selected_index.Peek());
                                    current = selected_index.Dequeue();
                                }
                                pool.Remove(current);
                                enqueued.AddFirst(current);
                                if (enqueued.Count > sets.Count - 3) // 
                                {
                                    pool.Add(enqueued.Last.Value);
                                    enqueued.RemoveLast();
                                }
                                set = sets[current];

                                UnturnedChat.Say("Current round: " + set.name + " - " + set.round_name);
                                for (int i = 0; i < Provider.clients.Count; i++)
                                {
                                    UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(Provider.clients[i].playerID.steamID);
                                    EquipPlayer(uPlayer);
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

        private void EquipPlayer(UnturnedPlayer player)
        {
            foreach (SetItem item in set.items)
            {
                player.GiveItem(item.item, item.amount);
            }
            player.Experience += 100; // give player 100 exp
        }
    }
    public class RandomizerConfiguration : IRocketPluginConfiguration
    {
        //[XmlArrayItem(ElementName = "set")]
        public List<RandomSet> random_sets;

        public void LoadDefaults()
        {
            random_sets = new List<RandomSet>()
            {
                new RandomSet ()
                {
                    name = "Ace only", round_name = "Angry farmers", items = new List<SetItem>()
                    {
                        new SetItem(107, 1), // weapon
                        new SetItem(108, 6), // ammo
                        new SetItem(394, 3), // bandages
                        new SetItem(242, 1), // clothes
                        new SetItem(243, 1),
                        new SetItem(244, 1),
                    }
                },
                new RandomSet() // battle for the last doughnut
                {
                    name = "Cobra only", round_name = "Battle for the last doughnut", items = new List<SetItem>()
                    {
                        new SetItem(99, 1), // weapon
                        new SetItem(100, 6), // ammo
                        new SetItem(394, 3), // bandages
                        new SetItem(10, 1), // clothes
                        new SetItem(223, 1),
                        new SetItem(224, 1),
                        new SetItem(225, 1)
                    }
                },
                new RandomSet()
                {
                    name = "Bat only", round_name = "Thug skirmish", items = new List<SetItem>()
                    {
                        new SetItem(105, 1), // weapon
                        new SetItem(394, 3), // bandages
                        new SetItem(11, 1), // clothes
                        new SetItem(167, 1),
                        new SetItem(2, 1),
                    }
                },

            };
        }
    }

    public class SetItem {
        public ushort item;
        public byte amount;

        public SetItem() { }

        public SetItem(ushort item, byte amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }

    public class RandomSet
    {
        public string name;
        public string round_name;
        public List<SetItem> items;

        public RandomSet() { }

        public RandomSet(params SetItem[] args)
        {
            foreach (SetItem item in args) {
                items.Add(item);
            }
        }
    }

    public class Rounds : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "rounds";

        public string Help => "List all existing rounds.";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rounds" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (Randomizer.sets.Count == 0)
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
    }
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
            UnturnedPlayer.FromName(caller.DisplayName).Player.sendBrowserRequest("Come join our discord!", "https://discord.gg/u6NWH37");
        }
    }

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