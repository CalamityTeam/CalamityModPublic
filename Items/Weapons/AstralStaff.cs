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
	public class AstralStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Staff");
			Tooltip.SetDefault("Summons astral stars from the sky");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 53;
	        item.crit += 25;
	        item.magic = true;
	        item.mana = 15;
	        item.width = 42;
	        item.height = 42;
	        item.useTime = 9;
	        item.useAnimation = 18;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
	        item.value = 350000;
	        item.rare = 7;
	        item.UseSound = SoundID.Item105;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("AstralStar");
	        item.shootSpeed = 15f;
	    }
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "AstralBar", 6);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
			float num72 = item.shootSpeed;
	    	Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
	    	float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
			float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
			if (player.gravDir == -1f)
			{
				num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
			}
			float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
			float num81 = num80;
			if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
			{
				num78 = (float)player.direction;
				num79 = 0f;
				num80 = num72;
			}
			else
			{
				num80 = num72 / num80;
			}
	    	num78 *= num80;
			num79 *= num80;
			int num107 = 2;
			if (Main.rand.Next(3) == 0)
			{
				num107++;
			}
			if (Main.rand.Next(3) == 0)
			{
				num107++;
			}
			for (int num108 = 0; num108 < num107; num108++)
			{
				vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
				vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
				vector2.Y -= (float)(100 * num108);
				num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
				num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
				if (num79 < 0f)
				{
					num79 *= -1f;
				}
				if (num79 < 20f)
				{
					num79 = 20f;
				}
				num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
				num80 = num72 / num80;
				num78 *= num80;
				num79 *= num80;
				float speedX4 = num78 + (float)Main.rand.Next(-30, 31) * 0.02f;
				float speedY5 = num79 + (float)Main.rand.Next(-30, 31) * 0.02f;
				int star = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[star].magic = true;
			}
			return false;
		}
	}
}