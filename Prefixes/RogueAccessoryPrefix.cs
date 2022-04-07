using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Prefixes
{
    public class QuietPrefix : RogueAccessoryPrefix
    {
        public QuietPrefix() : base(1.02f) { }
    }

    public class CloakedPrefix : RogueAccessoryPrefix
    {
        public CloakedPrefix() : base(1.04f) { }
    }

    public class CamouflagedPrefix : RogueAccessoryPrefix
    {
        public CamouflagedPrefix() : base(1.06f) { }
    }

    public class SilentPrefix : RogueAccessoryPrefix
    {
        public SilentPrefix() : base(1.08f) { }
    }

    public class RogueAccessoryPrefix : ModPrefix
    {
        internal float stealthGenBonus = 1f;
        public override PrefixCategory Category => PrefixCategory.Accessory;

        public RogueAccessoryPrefix() { }

        public RogueAccessoryPrefix(float stealthGenBonus = 1f)
        {
            this.stealthGenBonus = stealthGenBonus;
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
    }
}
