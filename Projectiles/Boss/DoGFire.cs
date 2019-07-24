﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class DoGFire : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Death Fire");
		}

		public override void SetDefaults()
		{
			projectile.width = 6;
			projectile.height = 6;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.extraUpdates = 3;
			cooldownSlot = 1;
		}

		public override void AI()
		{
			if (projectile.scale <= 1.5f)
			{
				projectile.scale *= 1.01f;
			}
			if (projectile.ai[1] == 0f)
			{
				if (projectile.timeLeft > 60)
				{
					projectile.timeLeft = 60;
				}
			}
			else if (projectile.ai[1] == 1f)
			{
				if (projectile.timeLeft > 12)
				{
					projectile.timeLeft = 12;
				}
			}
			else
			{
				if (projectile.timeLeft > 80)
				{
					projectile.timeLeft = 80;
				}
			}
			if (projectile.ai[0] > 5f)
			{
				float num296 = 1f;
				if (projectile.ai[0] == 6f)
				{
					num296 = 0.25f;
				}
				else if (projectile.ai[0] == 7f)
				{
					num296 = 0.5f;
				}
				else if (projectile.ai[0] == 8f)
				{
					num296 = 0.75f;
				}
				projectile.ai[0] += 1f;
				int num297 = 173;
				if (Main.rand.Next(2) == 0)
				{
					for (int num298 = 0; num298 < 1; num298++)
					{
						int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
						if (Main.rand.Next(3) == 0)
						{
							Main.dust[num299].noGravity = true;
							Main.dust[num299].scale *= 3f;
							Dust expr_DBEF_cp_0 = Main.dust[num299];
							expr_DBEF_cp_0.velocity.X = expr_DBEF_cp_0.velocity.X * 2f;
							Dust expr_DC0F_cp_0 = Main.dust[num299];
							expr_DC0F_cp_0.velocity.Y = expr_DC0F_cp_0.velocity.Y * 2f;
						}
						else
						{
							Main.dust[num299].scale *= 1.5f;
						}
						Dust expr_DC74_cp_0 = Main.dust[num299];
						expr_DC74_cp_0.velocity.X = expr_DC74_cp_0.velocity.X * 1.2f;
						Dust expr_DC94_cp_0 = Main.dust[num299];
						expr_DC94_cp_0.velocity.Y = expr_DC94_cp_0.velocity.Y * 1.2f;
						Main.dust[num299].scale *= num296;
						Main.dust[num299].velocity += projectile.velocity;
						if (!Main.dust[num299].noGravity)
						{
							Main.dust[num299].velocity *= 0.5f;
						}
					}
				}
			}
			else
			{
				projectile.ai[0] += 1f;
			}
			projectile.rotation += 0.3f * (float)projectile.direction;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("GodSlayerInferno"), 600);
			target.AddBuff(BuffID.Frostburn, 600, true);
			target.AddBuff(BuffID.Darkness, 600, true);
		}
	}
}