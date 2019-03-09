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
	public class StellarStriker : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stellar Striker");
			Tooltip.SetDefault("Summons a swarm of lunar flares from the sky on enemy hits");
		}

		public override void SetDefaults()
		{
			item.width = 86;
			item.damage = 640;
			item.melee = true;
			item.useAnimation = 25;
			item.useStyle = 1;
			item.useTime = 25;
			item.useTurn = true;
			item.knockBack = 7.75f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 86;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}
		
		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			if (target.type == NPCID.TargetDummy)
			{
				return;
			}
			Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 88);
			int i = Main.myPlayer;
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
			int num112 = 2;
			for (int num113 = 0; num113 < num112; num113++) 
			{
				vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
				vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
				vector2.Y -= (float)(100 * num113);
				num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X + (float)Main.rand.Next(-40, 41) * 0.03f;
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
				float num114 = num78;
				float num115 = num79 + (float)Main.rand.Next(-80, 81) * 0.02f;
				Projectile.NewProjectile(vector2.X, vector2.Y, num114, num115, 645, (int)((double)((float)item.damage * player.meleeDamage) * 0.7), knockback, i, 0f, (float)Main.rand.Next(3));
			}
		}
		
		public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	            Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 229);
	        }
	    }
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CometQuasher");
			recipe.AddIngredient(ItemID.LunarBar, 10);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
