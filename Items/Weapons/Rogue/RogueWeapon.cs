using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.Items.Weapons.Rogue
{
	public abstract class RogueWeapon : ModItem
	{
		public virtual void SafeSetDefaults()
		{
		}

		public float StealthStrikeDamage;

		public RogueWeapon()
		{
			StealthStrikeDamage = 1f;
		}

		public override ModItem Clone(Item itemClone)
		{
			RogueWeapon myClone = (RogueWeapon)base.Clone(itemClone);
			myClone.StealthStrikeDamage = StealthStrikeDamage;
			return myClone;
		}

		public override int ChoosePrefix(UnifiedRandom rand)
		{
			WeightedRandom<string> newPrefix = new WeightedRandom<string>();
			newPrefix.Add("Pointy", 1);
			newPrefix.Add("Sharp", 1);
			newPrefix.Add("Feathered", 1);
			newPrefix.Add("Sleek", 1);
			newPrefix.Add("Hefty", 1);
			newPrefix.Add("Mighty", 1);
			newPrefix.Add("Glorious", 1);
			newPrefix.Add("Serrated", 1);
			newPrefix.Add("Vicious", 1);
			newPrefix.Add("Lethal", 1);
			newPrefix.Add("Flawless", 1);
			newPrefix.Add("Radical", 1);
			newPrefix.Add("Blunt", 1);
			newPrefix.Add("Flimsy", 1);
			newPrefix.Add("Unbalanced", 1);
			newPrefix.Add("Atrocious", 1);
			return mod.GetPrefix(newPrefix).Type;
		}

		public override bool NewPreReforge()
		{
			StealthStrikeDamage = 1f;
			return true;
		}

		public override bool? PrefixChance(int pre, UnifiedRandom rand)
		{
			if (item.maxStack > 1)
			{
				return false;
			}
			return null;
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

		// Simply add the player's dedicated rogue damage.
		// Rogue weapons are internally throwing so they already benefit from throwing damage boosts.
		// 5E-06 to prevent downrounding is not needed anymore, added by TML itself
		public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
		{
			add += player.Calamity().throwingDamage - 1f;
			//Boost (or lower) the weapon's damage if it has a stealth strike available and an associated prefix
			if (player.Calamity().StealthStrikeAvailable() && item.prefix > 0)
			{
				mult += StealthStrikeDamage - 1f;
			}
		}

		// Simply add the player's dedicated rogue crit chance.
		// Rogue crit isn't boosted by Calamity universal crit boosts, so this won't double-add universal crit.
		public override void GetWeaponCrit(Player player, ref int crit)
		{
			crit += player.Calamity().throwingCrit;
		}

		public override float UseTimeMultiplier(Player player)
		{
			float rogueAS = 1f;
			if (player.Calamity().gloveOfPrecision)
				rogueAS -= 0.2f;
			if (player.Calamity().gloveOfRecklessness)
				rogueAS += 0.2f;
			if (player.Calamity().titanHeartMantle)
				rogueAS -= 0.15f;
			return rogueAS;
		}

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
			if (item.prefix > 0)
			{
				float ssDmgBoost = StealthStrikeDamage - 1f;
				if (ssDmgBoost > 0)
				{
					TooltipLine StealthBonus = new TooltipLine(mod, "PrefixSSDmg", "+" + Math.Round(ssDmgBoost * 100f) + "% stealth strike damage")
					{
						isModifier = true
					};
					tooltips.Add(StealthBonus);
				}
				else if (ssDmgBoost < 0)
				{
					TooltipLine StealthBonus = new TooltipLine(mod, "PrefixSSDmg", "-" + Math.Round(Math.Abs(ssDmgBoost) * 100f) + "% stealth strike damage")
					{
						isModifier = true,
						isModifierBad = true
					};
					tooltips.Add(StealthBonus);
				}
			}
		}

		public override bool ConsumeItem(Player player)
		{
			if (player.Calamity().throwingAmmoCost50)
			{
				if (Main.rand.Next(1, 101) > 50)
					return false;
			}
			if (player.Calamity().throwingAmmoCost55)
			{
				if (Main.rand.Next(1, 101) > 55)
					return false;
			}
			if (player.Calamity().throwingAmmoCost66)
			{
				if (Main.rand.Next(1, 101) > 66)
					return false;
			}
			if (player.Calamity().throwingAmmoCost75)
			{
				if (Main.rand.Next(1, 101) > 75)
					return false;
			}
			return true;
		}
	}
}
