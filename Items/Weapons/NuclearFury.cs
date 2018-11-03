using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons 
{
	public class NuclearFury : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nuclear Fury");
			Tooltip.SetDefault("Casts a torrent of cosmic typhoons");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 95;
	        item.magic = true;
	        item.mana = 13;
	        item.width = 28;
	        item.height = 30;
	        item.useTime = 25;
	        item.useAnimation = 25;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5;
	        item.value = 2500000;
	        item.UseSound = SoundID.Item84;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("NuclearFuryProjectile");
	        item.shootSpeed = 16f;
	    }

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
	    	float spread = 45f * 0.0174f;
	    	double startAngle = Math.Atan2(speedX, speedY)- spread/2;
	    	double deltaAngle = spread/8f;
	    	double offsetAngle;
	    	int i;
	    	for (i = 0; i < 4; i++ )
	    	{
	   			offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
	        	Projectile.NewProjectile(position.X, position.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), type, damage, knockBack, Main.myPlayer);
	        	Projectile.NewProjectile(position.X, position.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), type, damage, knockBack, Main.myPlayer);
	    	}
	    	return false;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.LunarBar, 5);
	        recipe.AddIngredient(ItemID.SoulofSight, 10);
	        recipe.AddIngredient(ItemID.UnicornHorn, 5);
	        recipe.AddIngredient(ItemID.RazorbladeTyphoon);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}