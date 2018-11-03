using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.AbyssWeapons
{
	public class TyphonsGreed : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Typhon's Greed");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 75;
	        item.melee = true;
	        item.width = 16;
	        item.height = 16;
	        item.useTime = 30;
	        item.useAnimation = 30;
	        item.useStyle = 5;
	        item.noMelee = true;
            item.noUseGraphic = true;
	        item.knockBack = 5f;
	        item.value = 500000;
	        item.rare = 7;
	        item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
	        item.autoReuse = true;
            item.channel = true;
	        item.shoot = mod.ProjectileType("TyphonsGreedStaff");
	        item.shootSpeed = 24f;
	    }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DepthCells", 30);
            recipe.AddIngredient(null, "Lumenite", 10);
            recipe.AddIngredient(null, "Tenebris", 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}