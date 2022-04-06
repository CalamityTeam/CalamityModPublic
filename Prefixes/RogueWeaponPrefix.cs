using CalamityMod.Items.Weapons.Rogue;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Prefixes
{
    public enum RoguePrefixType : byte
    {
        Pointy,
        Sharp,
        Feathered,
        Sleek,
        Hefty,
        Mighty,
        Glorious,
        Serrated,
        Vicious,
        Lethal,
        Flawless,
        Radical,
        Blunt,
        Flimsy,
        Unbalanced,
        Atrocious
    }

    public class PointyWeaponPrefix : RogueWeaponPrefix
    {
        public PointyWeaponPrefix() : base(1.1f, 1f, 0, 1f, 1f) { }
    }

    public class SharpWeaponPrefix : RogueWeaponPrefix
    {
        public SharpWeaponPrefix() : base(1.15f, 1f, 0, 1f, 1f) { }
    }

    public class FeatheredWeaponPrefix : RogueWeaponPrefix
    {
        public FeatheredWeaponPrefix() : base(1f, 0.85f, 0, 1.1f, 1f) { }
    }

    public class SleekWeaponPrefix : RogueWeaponPrefix
    {
        public SleekWeaponPrefix() : base(1f, 0.9f, 0, 1.15f, 1f) { }
    }

    public class HeftyWeaponPrefix : RogueWeaponPrefix
    {
        public HeftyWeaponPrefix() : base(1.1f, 1f, 0, 1f, 1.15f) { }
    }

    public class MightyWeaponPrefix : RogueWeaponPrefix
    {
        public MightyWeaponPrefix() : base(1.15f, 1f, 0, 1f, 1.05f) { }
    }

    public class GloriousWeaponPrefix : RogueWeaponPrefix
    {
        public GloriousWeaponPrefix() : base(1.1f, 0.95f, 0, 1f, 1f) { }
    }

    public class SerratedWeaponPrefix : RogueWeaponPrefix
    {
        public SerratedWeaponPrefix() : base(1.1f, 0.9f, 0, 1.05f, 1f) { }
    }

    public class ViciousWeaponPrefix : RogueWeaponPrefix
    {
        public ViciousWeaponPrefix() : base(1.1f, 0.95f, 0, 1.15f, 1f) { }
    }

    public class LethalWeaponPrefix : RogueWeaponPrefix
    {
        public LethalWeaponPrefix() : base(1.1f, 0.95f, 2, 1.05f, 1.05f) { }
    }

    public class FlawlessWeaponPrefix : RogueWeaponPrefix
    {
        public FlawlessWeaponPrefix() : base(1.15f, 0.9f, 5, 1.1f, 1.15f) { }
    }

    public class RadicalWeaponPrefix : RogueWeaponPrefix
    {
        public RadicalWeaponPrefix() : base(1.05f, 0.95f, 0, 1.05f, 0.9f) { }
    }

    public class BluntWeaponPrefix : RogueWeaponPrefix
    {
        public BluntWeaponPrefix() : base(0.85f, 1f, 0, 1f, 1f) { }
    }

    public class FlimsyWeaponPrefix : RogueWeaponPrefix
    {
        public FlimsyWeaponPrefix() : base(0.9f, 1f, 0, 1f, 0.9f) { }
    }

    public class UnbalancedWeaponPrefix : RogueWeaponPrefix
    {
        public UnbalancedWeaponPrefix() : base(1f, 1.15f, 0, 0.95f, 1f) { }
    }

    public class AtrociousWeaponPrefix : RogueWeaponPrefix
    {
        public AtrociousWeaponPrefix() : base(0.85f, 1f, 0, 0.9f, 0.9f) { }
    }

    public class RogueWeaponPrefix : ModPrefix
    {
        //Thank you Thomas for helping me set this up =D
        internal float damageMult = 1f;
        internal float useTimeMult = 1f;
        internal int critBonus = 0;
        internal float shootSpeedMult = 1f;
        internal float stealthDmgMult = 1f;

        public override PrefixCategory Category => PrefixCategory.Custom;

        public RogueWeaponPrefix()
        {

        }

        public RogueWeaponPrefix(float damageMult = 1f, float useTimeMult = 1f, int critBonus = 0, float shootSpeedMult = 1f, float stealthDmgMult = 1f)
        {
            this.damageMult = damageMult;
            this.useTimeMult = useTimeMult;
            this.critBonus = critBonus;
            this.shootSpeedMult = shootSpeedMult;
            this.stealthDmgMult = stealthDmgMult;
        }

        public override void Apply(Item item)
        {
            ModItem moddedItem = item.ModItem;
            if (moddedItem != null && moddedItem is RogueWeapon rogueWep)
            {
                rogueWep.StealthStrikePrefixBonus = stealthDmgMult;
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            float extraValue = 1f + (1f * (stealthDmgMult - 1f));
            valueMult *= extraValue;
        }

        public override bool CanRoll(Item item) => item.Calamity().rogue && item.maxStack == 1;

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = this.damageMult;
            useTimeMult = this.useTimeMult;
            critBonus = this.critBonus;
            shootSpeedMult = this.shootSpeedMult;
        }
    }
}
