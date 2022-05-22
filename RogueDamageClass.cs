using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class RogueDamageClass : DamageClass
    {
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
