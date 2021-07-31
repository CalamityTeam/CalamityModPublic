using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ChickenRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken Rocket");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.MaxUpdates = 2;
            projectile.timeLeft = 300;
            projectile.ranged = true;
        }

        public override void AI()
        {
            // Animation frames.
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            float speed = projectile.velocity.Length();
            if (speed >= 12f)
            {
                // If the rocket is going fast enough, emit some dust.
                for (int i = 0; i < 2; i++)
                {
                    float dx = i == 1 ? projectile.velocity.X * 0.5f : 0f;
                    float dy = i == 1 ? projectile.velocity.Y * 0.5f : 0f;
                    int d = Dust.NewDust(new Vector2(projectile.position.X + 3f + dx, projectile.position.Y + 3f + dy) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[d].scale *= 2f + Main.rand.NextFloat();
                    Main.dust[d].velocity *= 0.2f;
                    Main.dust[d].noGravity = true;
                    d = Dust.NewDust(new Vector2(projectile.position.X + 3f + dx, projectile.position.Y + 3f + dy) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 244, 0f, 0f, 100, default, 0.5f);
                    Main.dust[d].fadeIn = 1f + Main.rand.NextFloat(0.5f);
                    Main.dust[d].velocity *= 0.05f;
                }

                // Exponentially accelerate if not going fast enough yet.
                if (speed < 21f)
                    projectile.velocity *= 1.006f;

                // When going at very high speed, emit even more dust.
                else if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                    Main.dust[d].scale = 0.1f + Main.rand.NextFloat(0.5f);
                    Main.dust[d].fadeIn = 1.5f + Main.rand.NextFloat(0.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = projectile.Center + new Vector2(0f, (float)(-(float)projectile.height / 2)).RotatedBy(projectile.rotation) * 1.1f;
                    Main.rand.Next(2);
                    d = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[d].scale = 1f + Main.rand.NextFloat(0.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = projectile.Center + new Vector2(0f, (float)(-(float)projectile.height / 2 - 6)).RotatedBy(projectile.rotation) * 1.1f;
                }
            }

            projectile.ai[0] += 1f;
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            // Slight gravity, equivalent to Plasma Grenade.
            projectile.velocity.Y += 0.09f;
        }

        // Instead of dying instantly on collision, fly straight up for a moment.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity.X = 0f;
            projectile.velocity.Y = -15f;

            // If there isn't much time left anyway, just explode immediately on collision.
            if (projectile.timeLeft > 20)
                projectile.timeLeft = 20;
            else
                return true;

            return false;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 1040;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChickenExplosion>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
            Main.PlaySound(SoundID.Item14, (int)projectile.Center.X, (int)projectile.Center.Y);
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[d].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[d].scale = 0.5f;
                    Main.dust[d].fadeIn = 1f + Main.rand.NextFloat();
                }
            }
            for (int i = 0; i < 70; i++)
            {
                int d = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 5f;
                d = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[d].velocity *= 2f;
            }
        }


    }
}
