using Terraria.ModLoader;

// + Throwing
// '--+ Rogue
//    '--> Stealth

namespace CalamityMod
{
    // Rogue is a subclass of Throwing (the removed vanilla class re-added by TML).
    // It has no technical differences from Throwing, but is used by all Calamity thrown weapons.
    public class RogueDamageClass : DamageClass
    {
        internal static RogueDamageClass Instance;

        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Throwing || damageClass == Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass) =>
            damageClass == Throwing;

        // SetDefaultStats not included, because the player gets 4% Throwing crit by default, which carries over to this
    }

    // TODO -- actually implement stealth strikes to be Stealth class
    // Stealth is a subclass of Rogue which is used for stealth strikes.
    // Stat boosts to this class will specifically only apply to stealth strikes.
    // No weapons are Stealth class, only projectiles.
    // Stealth class damage calculations are shown in tooltips for convenience.
    public class StealthDamageClass : DamageClass
    {
        internal static StealthDamageClass Instance;

        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic || damageClass == Throwing || damageClass == RogueDamageClass.Instance)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        // Stealth inherits effects from Throwing and regular Rogue.
        public override bool GetEffectInheritance(DamageClass damageClass) =>
            damageClass == Throwing || damageClass == RogueDamageClass.Instance;

        // SetDefaultStats not included, because the player gets 4% Throwing crit by default, which carries over to this
    }
}
