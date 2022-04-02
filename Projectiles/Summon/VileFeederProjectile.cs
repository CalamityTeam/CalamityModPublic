using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class VileFeederProjectile : ModProjectile
    {
        private int bounce = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eater of Souls Mini");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.minion = true;
            projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 50;
            }
            else
            {
                projectile.extraUpdates = 0;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter >= 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 4)
            {
                projectile.frame = 0;
            }
            for (int num369 = 0; num369 < 1; num369++)
            {
                int dustType = 7;
                float num370 = projectile.velocity.X / 3f * (float)num369;
                float num371 = projectile.velocity.Y / 3f * (float)num369;
                int num372 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[num372];
                dust.position.X = projectile.Center.X - num370;
                dust.position.Y = projectile.Center.Y - num371;
                dust.velocity *= 0f;
                dust.scale = 0.5f;
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) - MathHelper.PiOver2;
            float num373 = projectile.position.X;
            float num374 = projectile.position.Y;
            float num375 = 100000f;
            bool flag10 = false;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 30f)
            {
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float num377 = npc.position.X + (float)(npc.width / 2);
                        float num378 = npc.position.Y + (float)(npc.height / 2);
                        float num379 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num377) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num378);
                        if (num379 < 640f && num379 < num375 && Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            num375 = num379;
                            num373 = num377;
                            num374 = num378;
                            flag10 = true;
                        }
                    }
                }
                if (!flag10)
                {
                    for (int num376 = 0; num376 < Main.maxNPCs; num376++)
                    {
                        NPC npc = Main.npc[num376];
                        if (npc.CanBeChasedBy(projectile, false))
                        {
                            float num377 = npc.position.X + (float)(npc.width / 2);
                            float num378 = npc.position.Y + (float)(npc.height / 2);
                            float num379 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num377) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num378);
                            if (num379 < 640f && num379 < num375 && Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                            {
                                num375 = num379;
                                num373 = num377;
                                num374 = num378;
                                flag10 = true;
                            }
                        }
                    }
                }
            }
            if (!flag10)
            {
                num373 = projectile.position.X + (float)(projectile.width / 2) + projectile.velocity.X * 100f;
                num374 = projectile.position.Y + (float)(projectile.height / 2) + projectile.velocity.Y * 100f;
            }
            float num380 = 10f;
            float num381 = 0.16f;
            Vector2 vector30 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num382 = num373 - vector30.X;
            float num383 = num374 - vector30.Y;
            float num384 = (float)Math.Sqrt((double)(num382 * num382 + num383 * num383));
            num384 = num380 / num384;
            num382 *= num384;
            num383 *= num384;
            if (projectile.velocity.X < num382)
            {
                projectile.velocity.X = projectile.velocity.X + num381;
                if (projectile.velocity.X < 0f && num382 > 0f)
                {
                    projectile.velocity.X = projectile.velocity.X + num381 * 2f;
                }
            }
            else if (projectile.velocity.X > num382)
            {
                projectile.velocity.X = projectile.velocity.X - num381;
                if (projectile.velocity.X > 0f && num382 < 0f)
                {
                    projectile.velocity.X = projectile.velocity.X - num381 * 2f;
                }
            }
            if (projectile.velocity.Y < num383)
            {
                projectile.velocity.Y = projectile.velocity.Y + num381;
                if (projectile.velocity.Y < 0f && num383 > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num381 * 2f;
                }
            }
            else if (projectile.velocity.Y > num383)
            {
                projectile.velocity.Y = projectile.velocity.Y - num381;
                if (projectile.velocity.Y > 0f && num383 < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num381 * 2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
            {
                projectile.Kill();
            }
            else
            {
                projectile.ai[0] += 15f;
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override bool CanDamage() => projectile.ai[0] >= 30f;
    }
}
