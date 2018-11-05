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
	public class DarkechoGreatbow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Darkecho Greatbow");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 37;
	        item.ranged = true;
	        item.width = 34;
	        item.height = 62;
	        item.useTime = 22;
	        item.useAnimation = 22;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 4;
	        item.value = 175000;
	        item.rare = 5;
	        item.UseSound = SoundID.Item5;
	        item.autoReuse = true;
	        item.shoot = 10;
	        item.shootSpeed = 16f;
	        item.useAmmo = 40;
	    }

        /*public void OverhaulInit()
        {
            this.SetTag("bow");
        }*/

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	for (int i = 0; i < 2; i++)
	    	{
	    		float SpeedX = speedX + (float) Main.rand.Next(-30, 31) * 0.05f;
	        	float SpeedY = speedY + (float) Main.rand.Next(-30, 31) * 0.05f;
	    		switch (Main.rand.Next(3))
				{
	    			case 1: type = ProjectileID.UnholyArrow; break;
	    			default: break;
				}
	        	int index = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
				Main.projectile[index].noDropItem = true;
	    	}
	    	return false;
		}
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "VerstaltiteBar", 8);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}