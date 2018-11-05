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
	public class BladedgeGreatbow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bladedge Railbow");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 26;
	        item.ranged = true;
	        item.width = 74;
	        item.height = 22;
	        item.useTime = 24;
	        item.useAnimation = 24;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.5f;
	        item.value = 200000;
	        item.rare = 6;
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

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	for (int i = 0; i < 5; i++)
	    	{
	            float SpeedX = speedX + (float) Main.rand.Next(-60, 61) * 0.05f;
	            float SpeedY = speedY + (float) Main.rand.Next(-60, 61) * 0.05f;
	    		switch (Main.rand.Next(5))
				{
	    			case 1: type = ProjectileID.ChlorophyteArrow; break;
	    			default: break;
				}
	        	int index = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
				Main.projectile[index].noDropItem = true;
	    	}
	    	return false;
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