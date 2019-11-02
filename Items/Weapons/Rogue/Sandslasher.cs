using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class Sandslasher : RogueWeapon
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandslasher");
			Tooltip.SetDefault("Throws a huge shuriken made out of fused sand unaffected by gravity which slowly accelerates horizontally\n"
							  +"It does more damage depending on how fast it goes horizontally and how long it has been flying for");
        }

		public override void SafeSetDefaults()
		{
			item.width = 40;
			item.height = 40;
			item.damage = 100;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.useTime = 20;
			item.knockBack = 5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<SandslasherProj>();
			item.shootSpeed = 15f;
            item.Calamity().rogue = true;
		}
		
		public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ModContent.ItemType<GrandScale>());
			recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 6);
	        recipe.AddIngredient(ItemID.GoldBar, 10);
            recipe.AddIngredient(ItemID.HardenedSand, 25);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
			recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ModContent.ItemType<GrandScale>());
			recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 6);
	        recipe.AddIngredient(ItemID.PlatinumBar, 10);
            recipe.AddIngredient(ItemID.HardenedSand, 25);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
