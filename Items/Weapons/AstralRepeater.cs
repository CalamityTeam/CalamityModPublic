using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
//using TerrariaOverhaul;

namespace CalamityMod.Items.Weapons 
{
	public class AstralRepeater : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Repeater");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 65;
	        item.crit += 25;
	        item.ranged = true;
	        item.width = 50;
	        item.height = 34;
	        item.useTime = 4;
	        item.reuseDelay = 15;
	        item.useAnimation = 12;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 2.5f;
	        item.value = 350000;
	        item.rare = 7;
	        item.UseSound = SoundID.Item5;
	        item.autoReuse = true;
	        item.shoot = 10;
	        item.shootSpeed = 16f;
	        item.useAmmo = 40;
	    }

        /*public void OverhaulInit()
        {
            this.SetTag("crossbow");
        }*/

        public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "AstralBar", 7);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}