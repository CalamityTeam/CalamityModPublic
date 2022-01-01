using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class UniversalGenesisStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Star");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 22;
			projectile.height = 20;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.alpha = 50;
			projectile.tileCollide = false;
			projectile.penetrate = 5;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
		}

		public override void AI()
		{
			if (projectile.soundDelay == 0 && projectile.ai[0] == 0f)
			{
				projectile.soundDelay = 20 + Main.rand.Next(40);
				if (Main.rand.NextBool(5))
				{
					Main.PlaySound(SoundID.Item9, projectile.position);
				}
			}
			projectile.alpha -= 15;
			int alphaMin = 150;
			if (projectile.Center.Y >= projectile.ai[1])
			{
				alphaMin = 0;
			}
			if (projectile.alpha < alphaMin)
			{
				projectile.alpha = alphaMin;
			}
			projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * projectile.direction;
			if (Main.rand.NextBool(48))
			{
				int idx = Gore.NewGore(projectile.Center, projectile.velocity * 0.2f, 16, 1f);
				Main.gore[idx].velocity *= 0.66f;
				Main.gore[idx].velocity += projectile.velocity * 0.3f;
			}
			if (projectile.ai[1] == 1f)
			{
				projectile.light = 0.9f;
				if (Main.rand.NextBool(10))
				{
					Dust.NewDust(projectile.position, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default, 1.2f);
				}
				if (Main.rand.NextBool(20))
				{
					Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
				}
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, projectile.alpha);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			projectile.position += projectile.Size;
			projectile.width = 50;
			projectile.height = 50;
			projectile.position -= projectile.Size;
			for (int i = 0; i < 5; i++)
			{
				int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 16, 0f, 0f, 100, default, 1.2f);
				Main.dust[idx].velocity *= 3f;
				if (Main.rand.NextBool(2))
				{
					Main.dust[idx].scale = 0.5f;
					Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
			for (int i = 0; i < 3; i++)
			{
				Gore.NewGore(projectile.position, projectile.velocity * 0.05f, Main.rand.Next(16, 18), 1f);
			}
		}
	}
}
