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
	public class Alluvion : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Alluvion");
			Tooltip.SetDefault("Moderate chance to fire sharks\n" +
	                   "Low chance to fire typhoon arrows\n" +
	                   "Typhoons deal MORE damage in Expert Mode\n" +
	        			"Fires a torrent of ten arrows at once");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 45;
	        item.ranged = true;
	        item.width = 44;
	        item.height = 58;
	        item.useTime = 9;
	        item.useAnimation = 18;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 4f;
	        item.value = 10000000;
	        item.UseSound = SoundID.Item5;
	        item.autoReuse = true;
	        item.shoot = 1;
	        item.shootSpeed = 17f;
	        item.useAmmo = 40;
	    }

        /*public void OverhaulInit()
        {
            this.SetTag("bow");
        }*/

        public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(43, 96, 222);
	            }
	        }
	    }
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
	    	float num117 = 0.314159274f;
			int num118 = 10;
			Vector2 vector7 = new Vector2(speedX, speedY);
			vector7.Normalize();
			vector7 *= 20f;
			bool flag11 = Collision.CanHit(vector2, 0, 0, vector2 + vector7, 0, 0);
			for (int num119 = 0; num119 < num118; num119++)
			{
				float num120 = (float)num119 - ((float)num118 - 1f) / 2f;
				Vector2 value9 = vector7.RotatedBy((double)(num117 * num120), default(Vector2));
				if (!flag11)
				{
					value9 -= vector7;
				}
                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    if (Main.rand.Next(12) == 0)
                    {
                        type = mod.ProjectileType("TorrentialArrow");
                    }
                    if (Main.rand.Next(25) == 0)
                    {
                        type = 408;
                    }
                    if (Main.rand.Next(100) == 0)
                    {
                        type = mod.ProjectileType("TyphoonArrow");
                    }
                    int num121 = Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX, speedY, type, (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
                    Main.projectile[num121].noDropItem = true;
                }
                else
                {
                    int num121 = Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX, speedY, type, (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
                    Main.projectile[num121].noDropItem = true;
                }
			}
			return false;
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "Monsoon");
	        recipe.AddIngredient(null, "Phantoplasm", 5);
	        recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "DepthCells", 10);
            recipe.AddIngredient(null, "Lumenite", 20);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}