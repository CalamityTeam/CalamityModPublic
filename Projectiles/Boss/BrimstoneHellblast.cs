using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class BrimstoneHellblast : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Hellblast");
			Main.projFrames[projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			projectile.width = 40;
			projectile.height = 40;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = 1;
			projectile.timeLeft = 255;
			projectile.alpha = 0;
			cooldownSlot = 1;
		}

		public override void AI()
		{
			projectile.frameCounter++;
			if (projectile.frameCounter > 12)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 3)
			{
				projectile.frame = 0;
			}
			projectile.alpha += 1;
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.9f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
			if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
			}
			projectile.velocity.X *= 1.03f;
			projectile.velocity.Y *= 1.03f;
			if (projectile.velocity.X < 0f)
			{
				projectile.spriteDirection = -1;
				projectile.rotation = (float)Math.Atan2((double)(-(double)projectile.velocity.Y), (double)(-(double)projectile.velocity.X));
			}
			else
			{
				projectile.spriteDirection = 1;
				projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = (projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(250, 50, 50, projectile.alpha);
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("AbyssalFlames"), 180);
			target.AddBuff(mod.BuffType("VulnerabilityHex"), 120, true);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
			for (int dust = 0; dust <= 5; dust++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, 0f, 0f);
			}
		}
	}
}
