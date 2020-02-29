using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DarkBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 56;
            projectile.aiStyle = 18;
            aiType = ProjectileID.DeathSickle;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.2f / 255f, 0f, (255 - projectile.alpha) * 0.6f / 255f);

			if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 12f)
			{
				projectile.velocity *= 1.1f;
			}
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.Frostburn, 240);
		}

		public override void Kill(int timeLeft)
		{
			for (int num105 = 0; num105 < 20; num105++)
			{
				int num102 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 56, 0f, 0f, 0, default, 1f);
				Main.dust[num102].noGravity = true;
				Main.dust[num102].velocity += projectile.velocity * 0.1f;
			}
			if (projectile.owner == Main.myPlayer)
			{
				for (int k = 0; k < 3; k++)
				{
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)Main.rand.Next(-35, 36) * 0.2f, (float)Main.rand.Next(-35, 36) * 0.2f, ModContent.ProjectileType<TinyCrystal>(),
					(int)((double)projectile.damage * 0.5), projectile.knockBack * 0.15f, Main.myPlayer, 0f, 0f);
				}
			}
		}
	}
}
