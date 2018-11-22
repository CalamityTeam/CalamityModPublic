using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class FrostsparkBullet : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frostspark Bullet");
			Tooltip.SetDefault("Has a chance to freeze enemies and explode into electricity\n" +
                "Enemies that are immune to being frozen take more damage from these bullets");
		}

		public override void SetDefaults()
		{
			item.damage = 11;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 1.25f;
			item.value = 600;
			item.rare = 5;
			item.shoot = mod.ProjectileType("FrostsparkBullet");
			item.shootSpeed = 14f;
			item.ammo = 97;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.MusketBall, 150);
			recipe.AddIngredient(null, "CryoBar");
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this, 150);
			recipe.AddRecipe();
		}
	}
}