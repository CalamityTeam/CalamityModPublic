using Terraria.ModLoader;

// + Melee
// |--> MeleeNoSpeed
// |
// '--+ TrueMelee
//    '--> TrueMeleeNoSpeed

namespace CalamityMod
{
    // True Melee is a subclass of Melee (that is, the one affected by melee speed).
    // You can give players regular melee speed and it will affect true melee weapons of this damage class.
    // If you want to give players attack speed which only affects true melee weapons, give them TrueMeleeDamageClass attack speed.
    public class TrueMeleeDamageClass : DamageClass
    {
        internal static TrueMeleeDamageClass Instance;

        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Melee || damageClass == Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        // Does not inherit effects from MeleeNoSpeed, because that class already inherits directly from Melee itself.
        public override bool GetEffectInheritance(DamageClass damageClass) => damageClass == Melee;

        // SetDefaultStats not included, because the player gets 4% Melee crit by default, which carries over to this
    }

    // True Melee No Speed is a subclass of True Melee that, like MeleeNoSpeed, is NOT affected by melee speed.
    // You should never give the player stat boosts to this class.
    // Simply give them stat boosts to TrueMeleeDamageClass. This class will inherit everything but melee speed.
    public class TrueMeleeNoSpeedDamageClass : DamageClass
    {
        internal static TrueMeleeNoSpeedDamageClass Instance;

        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;

        // This code lifted from the definition of MeleeNoSpeed, with the added change that it benefits from standard True Melee.
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic || damageClass == Melee || damageClass == TrueMeleeDamageClass.Instance)
                return StatInheritanceData.Full with { attackSpeedInheritance = 0 };

            return StatInheritanceData.None;
        }

        // True Melee No Speed inherits effects from regular Melee, MeleeNoSpeed and regular True Melee.
        public override bool GetEffectInheritance(DamageClass damageClass) =>
            damageClass == Melee || damageClass == MeleeNoSpeed || damageClass == TrueMeleeDamageClass.Instance;

        // SetDefaultStats not included, because the player gets 4% Melee crit by default, which carries over to this
    }
}
