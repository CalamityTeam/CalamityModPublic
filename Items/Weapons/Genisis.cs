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
	public class Genisis : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Genisis");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 52;
	        item.magic = true;
	        item.mana = 4;
	        item.width = 74;
	        item.height = 28;
	        item.useTime = 3;
	        item.useAnimation = 3;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 1.5f;
	        item.value = 10000000;
	        item.UseSound = SoundID.Item33;
	        item.autoReuse = true;
	        item.shootSpeed = 6f;
	        item.shoot = mod.ProjectileType("BigBeamofDeath");
	    }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
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
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
	        int num6 = 3;
	        float SpeedX = speedX + (float) Main.rand.Next(-20, 21) * 0.05f;
		    float SpeedY = speedY + (float) Main.rand.Next(-20, 21) * 0.05f;
	        for (int index = 0; index < num6; ++index)
	        {
	            int projectile = Projectile.NewProjectile(position.X, position.Y, SpeedX * 1.05f, SpeedY * 1.05f, 440, (int)((double)damage * 0.75f), knockBack, player.whoAmI, 0.0f, 0.0f);
	            Main.projectile[projectile].timeLeft = 120;
	        }
	        return false;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.LaserMachinegun);
	        recipe.AddIngredient(ItemID.LunarBar, 5);
	        recipe.AddIngredient(null, "BarofLife", 5);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}