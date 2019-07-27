using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Enemy
{
	public class MantisRing : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 3;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.knockBack = 3f;
			projectile.width = 72;
			projectile.height = 30;
			projectile.hostile = true;
			projectile.penetrate = 8;
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item1, projectile.Center);

			for (int i = 0; i < 60; i++)
			{
				float angle = MathHelper.TwoPi * Main.rand.NextFloat(0f, 1f);
				Vector2 angleVec = angle.ToRotationVector2();
				float distance = Main.rand.NextFloat(14f, 36f);
				Vector2 off = angleVec * distance;
				off.Y *= ((float)projectile.height / projectile.width);
				Vector2 pos = projectile.Center + off;
				Dust d = Dust.NewDustPerfect(pos, mod.DustType("AstralBlue"), angleVec * Main.rand.NextFloat(2f, 4f));
				d.customData = true;
			}
		}

		public override void AI()
		{
			projectile.velocity *= 1.01f;

			projectile.frameCounter++;
			if (projectile.frameCounter > 4)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
				if (projectile.frame > 2)
				{
					projectile.frame = 0;
				}
			}

			//Dust
			for (int i = 0; i < 4; i++)
			{
				float angle = MathHelper.TwoPi * Main.rand.NextFloat(0f, 1f);
				float distance = Main.rand.NextFloat(14f, 36f);
				Vector2 off = angle.ToRotationVector2() * distance;
				off.Y *= ((float)projectile.height / projectile.width);
				Vector2 pos = projectile.Center + off;
				Dust.NewDustPerfect(pos, mod.DustType("AstralBlue"), Vector2.Zero);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}
	}
}
