using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class DemonshadeHelm : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Demonshade Helm");
			Tooltip.SetDefault("30% increased damage and 15% increased critical strike chance, +10 max minions");
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.height = 18;
			item.value = Item.buyPrice(5, 0, 0, 0);
			item.defense = 50; //15
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 16;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == mod.ItemType("DemonshadeBreastplate") && legs.type == mod.ItemType("DemonshadeGreaves");
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
			player.armorEffectDrawOutlines = true;
		}

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "100% increased minion damage\n" +
				"All attacks inflict the demon flame debuff\n" +
				"Shadowbeams and demon scythes will fire down when you are hit\n" +
				"A friendly red devil follows you around\n" +
				"Press Y to enrage nearby enemies with a dark magic spell for 10 seconds\n" +
				"This makes them do 25% more damage but they also take 125% more damage";
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.dsSetBonus = true;
			if (player.whoAmI == Main.myPlayer && !modPlayer.chibii)
			{
				modPlayer.redDevil = true;
				if (player.FindBuffIndex(mod.BuffType("RedDevil")) == -1)
				{
					player.AddBuff(mod.BuffType("RedDevil"), 3600, true);
				}
				if (player.ownedProjectileCounts[mod.ProjectileType("RedDevil")] < 1)
				{
					Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("RedDevil"), 10000, 0f, Main.myPlayer, 0f, 0f);
				}
			}
			player.minionDamage += 1f;
		}

		public override void UpdateEquip(Player player)
		{
			player.maxMinions += 10;
			player.meleeDamage += 0.3f;
			CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.3f;
			player.rangedDamage += 0.3f;
			player.magicDamage += 0.3f;
			player.minionDamage += 0.3f;
			player.meleeCrit += 15;
			player.magicCrit += 15;
			player.rangedCrit += 15;
			CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 15;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ShadowspecBar", 8);
			recipe.AddTile(null, "DraedonsForge");
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}