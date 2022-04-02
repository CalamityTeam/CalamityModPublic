using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class Ancient : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.penetrate = 6;
            projectile.extraUpdates = 6;
            projectile.timeLeft = 151;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.6f, 0.5f, 0f);
            if (projectile.timeLeft % 30 == 0)
            {
                int numProj = 3;
                float randomSpread = Main.rand.NextFloat(3f, 18f);
                float rotation = MathHelper.ToRadians(randomSpread);
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = -1; i < numProj - 1; i++)
                    {
                        Vector2 perturbedSpeed = projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i));
                        Projectile.NewProjectile(projectile.Center, perturbedSpeed, ModContent.ProjectileType<Ancient2>(), (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner, 0f, projectile.ai[1]);
                    }
                }
            }
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            if (projectile.ai[0] > 4f && projectile.numUpdates % 2 == 0)
            {
                int dustType = 22;
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust = Main.dust[idx];
                if (Main.rand.NextBool(2))
                {
                    dust.noGravity = true;
                    dust.scale *= 2f;
                    dust.velocity.X *= 3f;
                    dust.velocity.Y *= 3f;
                }
                else
                {
                    dust.scale *= 1.25f;
                }
                dust.velocity.X *= 2f;
                dust.velocity.Y *= 2f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                if (Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                    dust.scale *= 1.5f;
                    dust.velocity.X *= 2f;
                    dust.velocity.Y *= 2f;
                }
                else
                {
                    dust.scale *= 1.1f;
                }
                dust.velocity.X *= 1.5f;
                dust.velocity.Y *= 1.5f;
            }
            projectile.ai[0] += 1f;
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

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
