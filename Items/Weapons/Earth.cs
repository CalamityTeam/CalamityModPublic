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
	public class Earth : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Earth");
			Tooltip.SetDefault("Has a chance to lower enemy defense by 50 when striking them\n" +
			           "Your attacks will heal you a lot\n" +
			           "Rains RGB meteors that explode into more meteors after a short time on enemy hits\n" +
			           "Ice meteors freeze enemies\n" +
			           "Flame meteors explode\n" +
			           "Green meteors spawn healing orbs");
		}

		public override void SetDefaults()
		{
			item.width = 92;
			item.damage = 300;
			item.melee = true;
			item.useAnimation = 16;
			item.useStyle = 1;
			item.useTime = 16;
			item.useTurn = true;
			item.knockBack = 9.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 104;
			item.value = 69696969;
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(255, 0, 255);
	            }
	        }
	    }
		
		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			if (target.type == NPCID.TargetDummy)
			{
				return;
			}
			if (Main.rand.Next(3) == 0)
			{
				target.defense -= 50;
			}
			int heal = Main.rand.Next(20, 69);
		    player.statLife += heal;
		   	player.HealEffect(heal);
			float num72 = 25f;
		   	Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
		   	float num78 = (float)Main.mouseX - Main.screenPosition.X - vector2.X;
			float num79 = (float)Main.mouseY - Main.screenPosition.Y - vector2.Y;
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
			int num107 = 3;
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
				float speedX4 = num78;
				float speedY5 = num79 + (float)Main.rand.Next(-180, 181) * 0.02f;
				Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, mod.ProjectileType("Earth"), damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(10));
			}
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "GrandGuardian");
			recipe.AddIngredient(null, "GalacticaBlade");
			recipe.AddIngredient(null, "ShadowspecBar", 5);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
