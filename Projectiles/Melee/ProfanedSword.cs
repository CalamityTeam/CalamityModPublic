using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ProfanedSword : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Profaned Sword");
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.aiStyle = 113;
			projectile.timeLeft = 600;
			aiType = 598;
			projectile.melee = true;
		}

		public override void AI()
		{
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
			if (Main.rand.Next(4) == 0)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
			if (projectile.spriteDirection == -1)
			{
				projectile.rotation -= 1.57f;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = 8;
			target.AddBuff(mod.BuffType("BrimstoneFlames"), 180);
		}
	}
}
