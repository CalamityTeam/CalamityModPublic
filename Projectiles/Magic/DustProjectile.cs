using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class DustProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dust");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 90;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.5f, 0.4f, 0.01f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
            if (projectile.ai[0] > 7f)
            {
                int dustType = 22;
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust = Main.dust[idx];
                if (Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                    dust.scale *= 2f;
                    dust.velocity.X *= 3f;
                    dust.velocity.Y *= 3f;
                }
                dust.velocity.X *= 1.5f;
                dust.velocity.Y *= 1.5f;
            }
            projectile.ai[0] += 1f;
            projectile.rotation += 0.3f * (float)projectile.direction;
            if (projectile.timeLeft < 30)
                projectile.Opacity = MathHelper.Lerp(0f, 1f, (float)projectile.timeLeft / 30f);
        }

        public override bool CanDamage() => projectile.timeLeft > 30;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);

            // Dust effects
            Circle dustCircle = new Circle(projectile.Center, projectile.width / 2);

            for (int i = 0; i < 20; i++)
            {
                // Dust
                Vector2 dustPos = dustCircle.RandomPointInCircle();
                if ((dustPos - projectile.Center).Length() > 48)
                {
                    int dustIndex = Dust.NewDust(dustPos, 1, 1, 22);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].fadeIn = 1f;
                    Vector2 dustVelocity = projectile.Center - Main.dust[dustIndex].position;
                    float distToCenter = dustVelocity.Length();
                    dustVelocity.Normalize();
                    dustVelocity = dustVelocity.RotatedBy(MathHelper.ToRadians(-90f));
                    dustVelocity *= distToCenter * 0.04f;
                    Main.dust[dustIndex].velocity = dustVelocity;
                }
            }
            return false;
        }
    }
}
