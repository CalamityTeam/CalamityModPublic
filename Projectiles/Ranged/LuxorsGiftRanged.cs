using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class LuxorsGiftRanged : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gift");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.height = 14;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.alpha = 255;
			projectile.timeLeft = 180;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			if (projectile.timeLeft < 85)
			{
				byte b2 = (byte)(projectile.timeLeft * 3);
				byte a2 = (byte)(100f * ((float)b2 / 255f));
				return new Color((int)b2, (int)b2, (int)b2, (int)a2);
			}
			return new Color(255, 255, 255, 100);
		}

		public override void AI()
		{
			if (projectile.localAI[0] == 0f)
			{
				projectile.scale -= 0.01f;
				projectile.alpha += 15;
				if (projectile.alpha >= 250)
				{
					projectile.alpha = 255;
					projectile.localAI[0] = 1f;
				}
			}
			else if (projectile.localAI[0] == 1f)
			{
				projectile.scale += 0.01f;
				projectile.alpha -= 15;
				if (projectile.alpha <= 0)
				{
					projectile.alpha = 0;
					projectile.localAI[0] = 0f;
				}
			}
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			projectile.localAI[1] += 1f;
			if (projectile.localAI[1] == 3f)
			{
				for (int l = 0; l < 12; l++)
				{
					Vector2 offset = Vector2.UnitX * (float)-(float)projectile.width / 2f;
					offset += -Vector2.UnitY.RotatedBy((double)(l * MathHelper.Pi / 6f), default) * new Vector2(8f, 16f);
					offset = offset.RotatedBy((double)(projectile.rotation - MathHelper.PiOver2), default);
					int electric = Dust.NewDust(projectile.Center, 0, 0, 135, 0f, 0f, 160, default, 1f);
					Main.dust[electric].scale = 1.1f;
					Main.dust[electric].noGravity = true;
					Main.dust[electric].position = projectile.Center + offset;
					Main.dust[electric].velocity = projectile.velocity * 0.1f;
					Main.dust[electric].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[electric].position) * 1.25f;
				}
			}
		}

		public override void Kill(int timeLeft)
		{
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 16);
			projectile.maxPenetrate = projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.Damage();
			Main.PlaySound(SoundID.Item92, projectile.position);
			int dustAmt = Main.rand.Next(10, 20);
			for (int d = 0; d < dustAmt; d++)
			{
				int electric = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, 135, 0f, 0f, 100, default, 2f);
				Main.dust[electric].velocity *= 2f;
				Main.dust[electric].noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
			return false;
		}
	}
}
