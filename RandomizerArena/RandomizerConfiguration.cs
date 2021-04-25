using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizerArena
{
    public class RandomizerConfiguration : IRocketPluginConfiguration
    {
        public int protection_duration;
        public bool maxskills;
        public uint start_experience;
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
            
        }
    }



    /*class AssetModification
    {
        public ushort asset_id;
        public bool disabled;

        public AssetModification(ushort asset_id)
        {
            this.asset_id = asset_id;
            disabled = false;
        }
    }

    class WeaponKitModification : AssetModification
    {
        List<Magazine> disabled_magazines;
        List<Sight> disabled_sights;

        public WeaponKitModification(ushort asset_id) : base(asset_id)
        {
            disabled_magazines = new List<Magazine>();
            disabled_sights = new List<Sight>();
        }
    }*/
}
