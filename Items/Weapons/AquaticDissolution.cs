using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class AquaticDissolution : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aquatic Dissolution");
			Tooltip.SetDefault("Fires aquatic jets from the sky that bounce off tiles");
		}

		public override void SetDefaults()
		{
			item.width = 50;
			item.damage = 154;
			item.melee = true;
			item.useAnimation = 16;
			item.useTime = 16;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 5.5f;
			item.UseSound = SoundID.Item60;
			item.autoReuse = true;
			item.height = 72;
			item.value = 10000000;
			item.shoot = mod.ProjectileType("OceanBeam");
			item.shootSpeed = 12f;
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
            for (int x = 0; x < 3; x++)
            {
                Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-100, 100), player.position.Y - 600f, 0f, 8f, type, damage, knockBack, Main.myPlayer, 0f, 0f);
            }
            return false;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Mariana");
			recipe.AddIngredient(null, "UeliaceBar", 7);
			recipe.AddIngredient(null, "BarofLife", 2);
            recipe.AddIngredient(null, "Lumenite", 20);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
			{
				int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 33, (float)(player.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
				Main.dust[num250].velocity *= 0.2f;
				Main.dust[num250].noGravity = true;
			}
	    }
	}
}
