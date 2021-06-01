using LiteDB;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;

namespace RandomizerArena
{
    public class RandomizerEconomy
    {
        private LiteDatabase db;
        private ILiteCollection<PlayerBalance> balance_table;

        public RandomizerEconomy()
        {
            db = new LiteDatabase(@"./Plugins/RandomizerArena/Database.db");
            balance_table = db.GetCollection<PlayerBalance>("balance");
            balance_table.EnsureIndex(x => x.steam_id);
        }

        public void RewardFrag(UnturnedPlayer player)
        {
            PlayerBalance pb = GetOrCreateEntry(player);
            pb.balance += 75;
            balance_table.Update(pb);
        }

        public void RewardAd(UnturnedPlayer player)
        {
            RewardAd(player.CSteamID.m_SteamID);
        }

        public void RewardAd(ulong steam_id)
        {
            PlayerBalance pb = GetOrCreateEntry(steam_id);
            pb.balance += 375;
            balance_table.Update(pb);
        }

        public bool PayForNominate(ulong steam_id)
        {
            PlayerBalance pb = GetOrCreateEntry(steam_id);
            if (pb.balance < 1000)
            {
                return false;
            }
            pb.balance -= 1000;
            balance_table.Update(pb);
            return true;
        }

        public void RefundNominate(ulong steam_id)
        {

        }

        public uint GetBalance(UnturnedPlayer player)
        {
            return GetOrCreateEntry(player).balance;
        }

        private PlayerBalance GetOrCreateEntry(UnturnedPlayer player)
        {
            PlayerBalance pb;
            if ((pb = balance_table.FindOne(x => x.steam_id == player.CSteamID.m_SteamID)) == null)
            {
                pb = new PlayerBalance
                {
                    steam_id = player.CSteamID.m_SteamID,
                    balance = 0
                };
                balance_table.Insert(pb);
            }
            return pb;
        }

        private PlayerBalance GetOrCreateEntry(ulong steam_id)
        {
            PlayerBalance pb;
            if ((pb = balance_table.FindOne(x => x.steam_id == steam_id)) == null)
            {
                pb = new PlayerBalance
                {
                    steam_id = steam_id,
                    balance = 0
                };
                balance_table.Insert(pb);
            }
            return pb;
        }
    }

    public class PlayerBalance
    {
        public ObjectId Id { get; set; }
        public ulong steam_id { get; set; }
        public uint balance { get; set; }

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
            if (player == null) return;
            if (murderer == null) return;
            UnturnedPlayer killer = UnturnedPlayer.FromCSteamID(murderer);
            if (killer == null) return;
            RandomizerArena.economy.RewardFrag(killer);
            UnturnedChat.Say(killer, "You got 75 credits for fragging " + player.CharacterName);
        }
    }
}
