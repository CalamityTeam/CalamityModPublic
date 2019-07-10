using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class BiomeBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Biome Blade");
			Tooltip.SetDefault("Fires different projectiles based on what biome you're in");
		}

		public override void SetDefaults()
		{
			item.width = 36;
			item.damage = 48;
			item.melee = true;
			item.useAnimation = 24;
			item.useTime = 24;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 6f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 36;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 4;
			item.shoot = mod.ProjectileType("BiomeOrb");
			item.shootSpeed = 12f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.WoodenSword);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddIngredient(ItemID.SandBlock, 10);
			recipe.AddIngredient(ItemID.IceBlock, 10);
			recipe.AddIngredient(ItemID.EbonstoneBlock, 10);
			recipe.AddIngredient(ItemID.GlowingMushroom, 10);
			recipe.AddIngredient(ItemID.Marble, 10);
			recipe.AddIngredient(ItemID.Granite, 10);
			recipe.AddIngredient(ItemID.Hellstone, 10);
			recipe.AddIngredient(ItemID.Coral, 5);
			recipe.AddIngredient(ItemID.PearlstoneBlock, 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.WoodenSword);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddIngredient(ItemID.SandBlock, 10);
			recipe.AddIngredient(ItemID.IceBlock, 10);
			recipe.AddIngredient(ItemID.CrimstoneBlock, 10);
			recipe.AddIngredient(ItemID.GlowingMushroom, 10);
			recipe.AddIngredient(ItemID.Marble, 10);
			recipe.AddIngredient(ItemID.Granite, 10);
			recipe.AddIngredient(ItemID.Hellstone, 10);
			recipe.AddIngredient(ItemID.Coral, 5);
			recipe.AddIngredient(ItemID.PearlstoneBlock, 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 0);
	        }
	    }
	}
}
