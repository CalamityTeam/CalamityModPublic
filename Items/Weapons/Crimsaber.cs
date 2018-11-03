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
    public class Crimsaber : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crimsaber");
			Tooltip.SetDefault("A sword fused with the blood of the gods");
		}

        public override void SetDefaults()
        {
            item.damage = 45;
            item.melee = true;
            item.width = 56;
            item.height = 70;
            item.useTime = 35;
            item.useAnimation = 25;
			item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 7;
            item.value = 150000;
            item.rare = 7;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shootSpeed = 8f;
        }
        
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimtaneBar, 20);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddIngredient(ItemID.Ichor, 5);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        
        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 60);
            if (target.type == NPCID.TargetDummy || target.type == NPCID.TheDestroyerBody)
            {
                return;
            }
            float num72 = item.shootSpeed;
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
			int num107 = 8;
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
				int piss = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, ProjectileID.GoldenShowerFriendly, damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(10));
                Main.projectile[piss].magic = false;
                Main.projectile[piss].melee = true;
            }
        }
        
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 5);
            }
        }
    }
}