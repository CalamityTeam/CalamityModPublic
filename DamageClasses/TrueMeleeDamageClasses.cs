using Terraria.ModLoader;

namespace CalamityMod
{
    // True Melee is a subclass of Melee, NOT MeleeNoSpeed. It is affected by melee speed.
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

        public override bool GetEffectInheritance(DamageClass damageClass) =>
            damageClass == Melee || damageClass == MeleeNoSpeed;

        // SetDefaultStats not included, because the player gets 4% Melee crit by default, which carries over to this
    }

    // True Melee is a subclass of MeleeNoSpeed, NOT Melee. It is NOT affected by melee speed.
    public class TrueMeleeNoSpeedDamageClass : DamageClass
    {
        internal static TrueMeleeNoSpeedDamageClass Instance;

        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == MeleeNoSpeed || damageClass == Generic)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass) =>
            damageClass == Melee || damageClass == MeleeNoSpeed;

        // SetDefaultStats not included, because the player gets 4% Melee crit by default, which carries over to this
    }
}
