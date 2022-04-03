using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Prefixes
{
    public class AccessoryPrefix : ModPrefix
    {
        //Thank you Thomas for helping me set this up =D
        internal static List<byte> AccessoryModifiers;
        internal float stealthGenBonus = 1f;

        public override PrefixCategory Category => PrefixCategory.Accessory;

        public AccessoryPrefix()
        {

        }

        public AccessoryPrefix(float stealthGenBonus = 1f)
        {
            this.stealthGenBonus = stealthGenBonus;
        }

        public override bool Autoload(ref string name)
        {
            if (base.Autoload(ref name))
            {
                AccessoryModifiers = new List<byte>();
                AddAccessoryPrefix(Mod, AccPrefixType.Quiet, 1.02f);
                AddAccessoryPrefix(Mod, AccPrefixType.Cloaked, 1.04f);
                AddAccessoryPrefix(Mod, AccPrefixType.Camouflaged, 1.06f);
                AddAccessoryPrefix(Mod, AccPrefixType.Silent, 1.08f);
            }
            return false;
        }

        public override void Apply(Item item)
        {
            item.Calamity().StealthGenBonus = stealthGenBonus;
        }

        public override void ModifyValue(ref float valueMult)
        {
            float extraValue = 1f + (2.5f * (stealthGenBonus - 1f));
            valueMult *= extraValue;
        }

        public override float RollChance(Item item)
        {
            if (item.vanity)
                return 0f;
            if (item.type == ItemID.GuideVoodooDoll || item.type == ItemID.MusicBox || item.type == ItemID.ClothierVoodooDoll)
                return 0f;
            if (item.type >= ItemID.MusicBoxOverworldDay && item.type <= ItemID.MusicBoxBoss3)
                return 0f;
            if (item.type >= ItemID.MusicBoxSnow && item.type <= ItemID.MusicBoxMushrooms)
                return 0f;

            return 1f;
        }

        private static void AddAccessoryPrefix(Mod mod, AccPrefixType prefixType, float stealthGenBonus = 1f)
        {
            string name = prefixType.ToString();
            mod.AddPrefix(name, new AccessoryPrefix(stealthGenBonus));
            AccessoryModifiers.Add(mod.GetPrefix(name).Type);
        }
    }

    public enum AccPrefixType : byte
    {
        Quiet,
        Cloaked,
        Camouflaged,
        Silent
    }
}
