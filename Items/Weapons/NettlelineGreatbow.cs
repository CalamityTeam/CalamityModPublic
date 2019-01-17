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
	public class NettlelineGreatbow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nettlevine Greatbow");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 120;
	        item.ranged = true;
	        item.width = 36;
	        item.height = 64;
	        item.useTime = 17;
	        item.useAnimation = 17;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
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

        public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 200);
	            }
	        }
	    }
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	        for(int i = 0; i < 5; i++)
	        {
	        	float SpeedX = speedX + (float) Main.rand.Next(-40, 41) * 0.05f;
	        	float SpeedY = speedY + (float) Main.rand.Next(-40, 41) * 0.05f;
                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    switch (Main.rand.Next(4))
                    {
                        case 1: type = ProjectileID.VenomArrow; break;
                        case 2: type = ProjectileID.ChlorophyteArrow; break;
                        default: break;
                    }
                    int index = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
                    Main.projectile[index].noDropItem = true;
                }
                else
                {
                    int num121 = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
                    Main.projectile[num121].noDropItem = true;
                }
            }
	    	return false;
		}
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "UeliaceBar", 12);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}