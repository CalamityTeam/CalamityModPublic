﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class BrimstoneFireblast : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Gigablast");
			Main.projFrames[projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			projectile.width = 50;
			projectile.height = 50;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = 1;
			projectile.alpha = 120;
			cooldownSlot = 1;
		}

		public override void AI()
		{
			if (projectile.ai[0] == 1f)
			{
				projectile.tileCollide = true;
			}
			projectile.frameCounter++;
			if (projectile.frameCounter > 4)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 5)
			{
				projectile.frame = 0;
			}
			projectile.alpha -= 1;
			if (projectile.alpha <= 0)
			{
				projectile.Kill();
			}
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.9f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
			}
			int num103 = (int)Player.FindClosest(projectile.Center, 1, 1);
			projectile.ai[1] += 1f;
			if (projectile.ai[1] < 220f && projectile.ai[1] > 20f)
			{
				float scaleFactor2 = projectile.velocity.Length();
				Vector2 vector11 = Main.player[num103].Center - projectile.Center;
				vector11.Normalize();
				vector11 *= scaleFactor2;
				projectile.velocity = (projectile.velocity * 24f + vector11) / 25f;
				projectile.velocity.Normalize();
				projectile.velocity *= scaleFactor2;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(250, 50, 50, projectile.alpha);
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("AbyssalFlames"), 240);
			target.AddBuff(mod.BuffType("VulnerabilityHex"), 180, true);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
			float spread = 12f * 0.0174f;
			double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
			double deltaAngle = spread / 30f;
			double offsetAngle;
			int i;
			if (projectile.owner == Main.myPlayer)
			{
				for (i = 0; i < 30; i++)
				{
					offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), mod.ProjectileType("BrimstoneBarrage"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 1f);
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), mod.ProjectileType("BrimstoneBarrage"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 1f);
				}
			}
			for (int dust = 0; dust <= 10; dust++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, 0f, 0f);
			}
		}
	}
}