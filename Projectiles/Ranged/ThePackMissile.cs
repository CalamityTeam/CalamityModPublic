using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ThePackMissile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pack Missile");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            Vector2 targetCenter = projectile.Center;
            float minTargetDistance = 2500f;
            bool homeIn = false;
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                {
                    float distanceFromTarget = projectile.Center.ManhattanDistance(Main.npc[i].Center);
                    if (distanceFromTarget < 200f)
                    {
                        if (projectile.owner == Main.myPlayer)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(6f, 12f);
                                Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<ThePackMinissile>(), (int)(projectile.damage * 0.25), projectile.knockBack, projectile.owner, 0f, 0f);
                            }
                        }
                        Main.PlaySound(SoundID.Item14, projectile.position);
                        projectile.Kill();
                        return;
                    }
                    else if (distanceFromTarget < minTargetDistance)
                    {
                        minTargetDistance = distanceFromTarget;
                        targetCenter = Main.npc[i].Center;
                        homeIn = true;
                    }
                }
            }
            if (homeIn)
            {
                projectile.velocity = (projectile.velocity * 15f + projectile.SafeDirectionTo(targetCenter) * 30f) / 16f;
                return;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 400;
            projectile.height = 400;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 40; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 255, 0f, 0f, 0, default, 1.5f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 60; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 255, 0f, 0f, 0, default, 2f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 255, 0f, 0f, 0, default, 1.5f);
                Main.dust[num624].velocity *= 2f;
            }
            projectile.Damage();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
