using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace CalamityMod.Items.Weapons.Rogue
{
    public abstract class RogueWeapon : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Rogue";
        // Apparently custom damage classes for weapons still don't allow for generic weapon prefixes
        public override bool WeaponPrefix() => true;

        public override bool RangedPrefix() => false;

        public override void ModifyResearchSorting(ref ItemGroup itemGroup) => itemGroup = (ItemGroup)CalamityResearchSorting.RogueWeapon;

        public virtual float StealthDamageMultiplier => 1f;
        public virtual float StealthVelocityMultiplier => 1f;
        public virtual float StealthKnockbackMultiplier => 1f;
        public virtual bool AdditionalStealthCheck() => false;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            bool stealthStrike = player.Calamity().StealthStrikeAvailable();
            if (stealthStrike || AdditionalStealthCheck())
			{
                damage = (int)(damage * StealthDamageMultiplier);
                velocity = velocity * StealthVelocityMultiplier;
                knockback = knockback * StealthKnockbackMultiplier;
			}

			ModifyStatsExtra(player, ref position, ref velocity, ref type, ref damage, ref knockback);
		}

        public virtual void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }
    }
}
