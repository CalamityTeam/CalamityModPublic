using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VenusianFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Venusian Cinder");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 120;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            if (projectile.velocity.X != projectile.velocity.X)
            {
                projectile.velocity.X *= -0.1f;
            }
            if (projectile.velocity.X != projectile.velocity.X)
            {
                projectile.velocity.X *= -0.5f;
            }
            if (projectile.velocity.Y != projectile.velocity.Y && projectile.velocity.Y > 1f)
            {
                projectile.velocity.Y *= -0.5f;
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 5f)
            {
                projectile.ai[0] = 5f;
                if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                {
                    projectile.velocity.X *= 0.97f;
                    if (projectile.velocity.X > -0.01f && projectile.velocity.X < 0.01f)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y += 0.2f;
            }
            projectile.rotation += projectile.velocity.X * 0.1f;
            if (projectile.ai[1] == 0f && projectile.type >= 326 && projectile.type <= 328)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 13);
            }
			if (Main.rand.NextBool(2))
			{
				int num199 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 100, default, 1f);
				Dust dust = Main.dust[num199];
				dust.position.X -= 2f;
				dust.position.Y += 2f;
				dust.scale += (float)Main.rand.Next(50) * 0.01f;
				dust.noGravity = true;
				dust.velocity.Y -= 2f;
			}
            if (Main.rand.NextBool(3))
            {
                int num200 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 100, default, 1f);
                Dust dust2 = Main.dust[num200];
                dust2.position.X -= 2f;
                dust2.position.Y += 2f;
                dust2.scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                dust2.noGravity = true;
                dust2.velocity *= 0.1f;
            }
            if (projectile.velocity.Y < 0.25f && projectile.velocity.Y > 0.15f)
            {
                projectile.velocity.X *= 0.8f;
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
