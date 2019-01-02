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
	public class Galeforce : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Galeforce");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 18;
	        item.ranged = true;
	        item.width = 32;
	        item.height = 52;
	        item.useTime = 17;
	        item.useAnimation = 17;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 4;
	        item.value = 75000;
	        item.rare = 3;
	        item.UseSound = SoundID.Item5;
	        item.autoReuse = true;
	        item.shoot = 10;
	        item.shootSpeed = 20f;
	        item.useAmmo = 40;
	    }

        /*public void OverhaulInit()
        {
            this.SetTag("bow");
        }*/

        public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "AerialiteBar", 8);
	        recipe.AddIngredient(ItemID.SunplateBlock, 3);
	        recipe.AddTile(TileID.SkyMill);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}