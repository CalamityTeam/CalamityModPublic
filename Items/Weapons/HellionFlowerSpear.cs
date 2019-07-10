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
	public class HellionFlowerSpear : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hellion Flower Spear");
			Tooltip.SetDefault("Shoots a flower spear tip\n" +
				"Summons petals from the sky on critical hits");
		}

		public override void SetDefaults()
		{
			item.width = 74;
			item.damage = 67;
			item.melee = true;
			item.noMelee = true;
			item.useTurn = true;
			item.noUseGraphic = true;
			item.useAnimation = 20;
			item.useStyle = 5;
			item.useTime = 20;
			item.knockBack = 7.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = false;
			item.height = 74;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
			item.shoot = mod.ProjectileType("HellionFlowerSpearProjectile");
			item.shootSpeed = 8f;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DraedonBar", 12);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
