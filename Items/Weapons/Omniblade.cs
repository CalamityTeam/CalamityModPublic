using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class Omniblade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Omniblade");
			Tooltip.SetDefault("An ancient blade forged by the legendary Omnir\n" +
			                   "Has a 20% chance to give enemies the whispering death debuff on hit\n" +
			                   "This debuff cuts enemy defense by 50 points");
		}

		public override void SetDefaults()
		{
			item.width = 100;
			item.damage = 84;
			item.crit += 45;
			item.melee = true;
			item.useAnimation = 10;
			item.useStyle = 1;
			item.useTime = 10;
			item.useTurn = true;
			item.knockBack = 6f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 120;
			item.value = 1500000;
			item.rare = 8;
		}
		
		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			if (Main.rand.Next(5) == 0)
			{
				target.AddBuff(mod.BuffType("WhisperingDeath"), 360);
			}
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Katana);
            recipe.AddIngredient(null, "BarofLife", 20);
			recipe.AddIngredient(null, "CoreofCalamity", 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
