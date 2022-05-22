using Terraria.ModLoader;

namespace CalamityMod
{
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
}
