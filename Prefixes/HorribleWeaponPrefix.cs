using System.Collections.Generic;
using CalamityMod.Items;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Prefixes
{
    
    public class Horrible : HorribleWeaponPrefix
    {
        public override float damageMult => 0.66f;
        public override float sizeMult => 2f;
    }
    public abstract class HorribleWeaponPrefix : ModPrefix, ILocalizedModType
    {
        public new string LocalizationCategory => "Prefixes.Weapon";

        // Stats
        public virtual float damageMult => 1f;
        public virtual float sizeMult => 1f;

        public override PrefixCategory Category => PrefixCategory.AnyWeapon;
        public override float RollChance(Item item) => 0; // Is manually applied by an item, can't be rerolled normally
        public override bool CanRoll(Item item)
        {
            return item.DamageType != null && item.DamageType != DamageClass.Default;
        }
        // Applying normal weapon stats
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = this.damageMult;
            scaleMult = this.sizeMult;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 2.5f;
        }
    }
}
