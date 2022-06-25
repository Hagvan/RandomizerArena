using LiteDB;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace RandomizerArena
{
    public class RandomizerData
    {
        private LiteDatabase db;
        private ILiteCollection<PlayerData> data_table;

        public RandomizerData()
        {
            db = new LiteDatabase(@"./Plugins/RandomizerArena/Database.db");
            data_table = db.GetCollection<PlayerData>("balance");
            data_table.EnsureIndex(x => x.steam_id);
        }

        public void RewardFrag(UnturnedPlayer player)
        {
            PlayerData pb = GetOrCreateEntry(player);
            pb.balance += 75;
            pb.score += 10;
            data_table.Update(pb);
        }

        public void RewardAd(UnturnedPlayer player)
        {
            RewardAd(player.CSteamID.m_SteamID);
        }

        public void RewardAd(ulong steam_id)
        {
            PlayerData pb = GetOrCreateEntry(steam_id);
            pb.balance += 375;
            data_table.Update(pb);
        }

        public bool PayForNominate(ulong steam_id)
        {
            PlayerData pb = GetOrCreateEntry(steam_id);
            if (pb.balance < 1500)
            {
                return false;
            }
            pb.balance -= 1500;
            data_table.Update(pb);
            return true;
        }

        public uint GetBalance(UnturnedPlayer player)
        {
            return GetOrCreateEntry(player).balance;
        }

        private PlayerData GetOrCreateEntry(UnturnedPlayer player)
        {
            PlayerData pb;
            if ((pb = data_table.FindOne(x => x.steam_id == player.CSteamID.m_SteamID)) == null)
            {
                pb = new PlayerData
                {
                    steam_id = player.CSteamID.m_SteamID,
                    balance = 0,
                    score = 0
                };
                data_table.Insert(pb);
            }
            return pb;
        }

        private PlayerData GetOrCreateEntry(ulong steam_id)
        {
            PlayerData pb;
            if ((pb = data_table.FindOne(x => x.steam_id == steam_id)) == null)
            {
                pb = new PlayerData
                {
                    steam_id = steam_id,
                    balance = 0,
                    score = 0
                };
                data_table.Insert(pb);
            }
            return pb;
        }
    }

    public class PlayerData
    {
        public ObjectId Id { get; set; }
        public ulong steam_id { get; set; }
        public uint balance { get; set; }
        public uint score { get; set; }
    }

    public class PlayerRA : UnturnedPlayerComponent
    {
        private UnturnedPlayerEvents rpe;

        protected void Start()
        {
            rpe = gameObject.transform.GetComponent<UnturnedPlayerEvents>();
            rpe.OnDeath += new UnturnedPlayerEvents.PlayerDeath(rpe_OnPlayerDeath);
        }
        private void rpe_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            try
            {
                UnturnedPlayer killer = UnturnedPlayer.FromCSteamID(murderer);
                if (player.Equals(killer)) return; // don't pay for suicide
                RandomizerArena.economy.RewardFrag(killer);
                UnturnedChat.Say(killer, "You got 75 credits for fragging " + player.CharacterName);
                foreach (SteamPlayer p in Provider.clients)
                {
                    UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromSteamPlayer(p);
                    if (unturnedPlayer.CSteamID != player.CSteamID || unturnedPlayer.CSteamID != killer.CSteamID)
                    {
                        UnturnedChat.Say(UnturnedPlayer.FromSteamPlayer(p), killer.CharacterName + " fragged " + player.CharacterName);
                    }
                }
            }
            catch
            {
                Logger.Log(player + " | " + cause + " | " + limb + " | " + murderer);
            }
        }
    }
}
