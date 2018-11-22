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
	public class BalefulHarvester : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Baleful Harvester");
		}

		public override void SetDefaults()
		{
			item.damage = 110;
			item.width = 66;
			item.height = 66;
			item.melee = true;
			item.useAnimation = 22;
			item.useStyle = 1;
			item.useTime = 22;
			item.useTurn = true;
			item.knockBack = 8;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.value = 475000;
			item.rare = 7;
			item.shoot = mod.ProjectileType("BalefulHarvesterProjectile");
			item.shootSpeed = 6f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Pumpkin, 30);
			recipe.AddIngredient(ItemID.BookofSkulls);
	        recipe.AddIngredient(ItemID.SpookyWood, 200);
	        recipe.AddIngredient(ItemID.TheHorsemansBlade);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
		
		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(BuffID.OnFire, 2400);
		}
	}
}
