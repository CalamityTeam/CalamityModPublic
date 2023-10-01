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
        public override string Name => "Pointy";
        public PointyWeaponPrefix() : base(1.1f, 1f, 0, 1f, 1f) { }
    }

    public class SharpWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Sharp";
        public SharpWeaponPrefix() : base(1.15f, 1f, 0, 1f, 1f) { }
    }

    public class FeatheredWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Feathered";
        public FeatheredWeaponPrefix() : base(1f, 0.85f, 0, 1.1f, 1f) { }
    }

    public class SleekWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Sleek";
        public SleekWeaponPrefix() : base(1f, 0.9f, 0, 1.15f, 1f) { }
    }

    public class HeftyWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Hefty";
        public HeftyWeaponPrefix() : base(1.1f, 1f, 0, 1f, 1.15f) { }
    }

    public class MightyWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Mighty";
        public MightyWeaponPrefix() : base(1.15f, 1f, 0, 1f, 1.05f) { }
    }

    public class GloriousWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Glorious";
        public GloriousWeaponPrefix() : base(1.1f, 0.95f, 0, 1f, 1f) { }
    }

    public class SerratedWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Serrated";
        public SerratedWeaponPrefix() : base(1.1f, 0.9f, 0, 1.05f, 1f) { }
    }

    public class ViciousWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Vicious";
        public ViciousWeaponPrefix() : base(1.1f, 0.95f, 0, 1.15f, 1f) { }
    }

    public class LethalWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Lethal";
        public LethalWeaponPrefix() : base(1.1f, 0.95f, 2, 1.05f, 1.05f) { }
    }

    public class FlawlessWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Flawless";
        public FlawlessWeaponPrefix() : base(1.15f, 0.9f, 5, 1.1f, 1.15f) { }
    }

    public class RadicalWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Radical";
        public RadicalWeaponPrefix() : base(1.05f, 0.95f, 0, 1.05f, 0.9f) { }
    }

    public class BluntWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Blunt";
        public BluntWeaponPrefix() : base(0.85f, 1f, 0, 1f, 1f) { }
    }

    public class FlimsyWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Flimsy";
        public FlimsyWeaponPrefix() : base(0.9f, 1f, 0, 1f, 0.9f) { }
    }

    public class UnbalancedWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Unbalanced";
        public UnbalancedWeaponPrefix() : base(1f, 1.15f, 0, 0.95f, 1f) { }
    }

    public class AtrociousWeaponPrefix : RogueWeaponPrefix
    {
        public override string Name => "Atrocious";
        public AtrociousWeaponPrefix() : base(0.85f, 1f, 0, 0.9f, 0.9f) { }
    }

    public class RogueWeaponPrefix : ModPrefix
    {
        internal float damageMult = 1f;
        internal float useTimeMult = 1f;
        internal int critBonus = 0;
        internal float shootSpeedMult = 1f;
        internal float stealthDmgMult = 1f;

        // The part where it can only be rolled by rogue weapons is done by CanRoll below
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;

        public RogueWeaponPrefix() { }

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
            if (item.CountsAsClass<RogueDamageClass>())
                item.Calamity().StealthStrikePrefixBonus = stealthDmgMult;
        }

        public override void ModifyValue(ref float valueMult)
        {
            float extraStealthDamage = stealthDmgMult - 1f;
            float stealthDamageValueMultiplier = 1f;
            float extraValue = 1f + stealthDamageValueMultiplier * extraStealthDamage;
            valueMult *= extraValue;
        }

        public override bool CanRoll(Item item) => item.CountsAsClass<ThrowingDamageClass>() && (item.maxStack == 1 || item.AllowReforgeForStackableItem) && GetType() != typeof(RogueWeaponPrefix);

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = this.damageMult;
            useTimeMult = this.useTimeMult;
            critBonus = this.critBonus;
            shootSpeedMult = this.shootSpeedMult;
        }
    }
}
