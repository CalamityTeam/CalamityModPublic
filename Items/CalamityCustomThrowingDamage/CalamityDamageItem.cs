using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public abstract class CalamityDamageItem : ModItem
	{
		public virtual void SafeSetDefaults()
		{
		}

		public sealed override void SetDefaults()
		{
			SafeSetDefaults();
			item.melee = false;
			item.ranged = false;
			item.magic = false;
			item.thrown = true;
			item.summon = false;
		}

		// 5E-06 to prevent downrounding is not needed anymore, added by TML itself
		public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
		{
			add += CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage - 1f;
		}

		// Calculates a rogue weapon's crit chance using the player's rogue crit chance.
		public override void GetWeaponCrit(Player player, ref int crit)
		{
			crit = (item.crit + CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit);
		}

		// Not needed because the weapon is now internally thrown instead of ranged
		/*
		public override void GetWeaponKnockback(Player player, ref float knockback)
		{
			if (player.shroomiteStealth)
			{
				knockback /= 1f + (1f - player.stealth) * 0.5f;
			}
			if (player.setVortex)
			{
				knockback /= 1f + (1f - player.stealth) * 0.5f;
			}
		}
		*/

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.mod == "Terraria");
			if (tt != null)
			{
				string[] splitText = tt.text.Split(' ');
				string damageValue = splitText.First();
				string damageWord = splitText.Last();
				tt.text = damageValue + " rogue " + damageWord;
			}
		}

		public override bool ConsumeItem(Player player)
		{
			if (CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingAmmoCost50)
			{
				if (Main.rand.Next(1, 101) > 50)
					return false;
			}
			if (CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingAmmoCost66)
			{
				if (Main.rand.Next(1, 101) > 66)
					return false;
			}
			return true;
		}
	}
}
