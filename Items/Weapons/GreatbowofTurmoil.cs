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
	public class GreatbowofTurmoil : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Greatbow of Turmoil");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 52;
	        item.ranged = true;
	        item.width = 18;
	        item.height = 36;
	        item.useTime = 17;
	        item.useAnimation = 17;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 4f;
	        item.value = 300000;
	        item.rare = 8;
	        item.UseSound = SoundID.Item5;
	        item.autoReuse = true;
	        item.shoot = 10;
	        item.shootSpeed = 17f;
	        item.useAmmo = 40;
	    }

        /*public void OverhaulInit()
        {
            this.SetTag("bow");
        }*/

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	for (int i = 0; i < 3; i++)
	    	{
		    	float SpeedX = speedX + (float) Main.rand.Next(-30, 31) * 0.05f;
		       	float SpeedY = speedY + (float) Main.rand.Next(-30, 31) * 0.05f;
		    	switch (Main.rand.Next(6))
				{
		    		case 1: type = ProjectileID.CursedArrow; break;
		    		case 2: type = ProjectileID.HellfireArrow; break;
		    		case 3: type = ProjectileID.IchorArrow; break;
		    		default: break;
				}
		        int index = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
				Main.projectile[index].noDropItem = true;
	    	}
	    	return false;
		}
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "CruptixBar", 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}