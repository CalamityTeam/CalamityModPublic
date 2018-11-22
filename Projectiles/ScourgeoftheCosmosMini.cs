using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class ScourgeoftheCosmosMini : ModProjectile
    {
        public int bounce = 3;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Scourge Mini");
            Main.projFrames[projectile.type] = 2;
        }
    	
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.melee = true;
            projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 1f)
            {
                projectile.thrown = true;
                projectile.melee = false;
            }
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
            if (projectile.frame >= 1)
            {
                projectile.frame = 0;
            }
            int num3;
            for (int num369 = 0; num369 < 1; num369 = num3 + 1)
            {
                int dustType = (Main.rand.Next(2) == 0 ? 56 : 242);
                float num370 = projectile.velocity.X / 3f * (float)num369;
                float num371 = projectile.velocity.Y / 3f * (float)num369;
                int num372 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, default(Color), 1f);
                Main.dust[num372].position.X = projectile.Center.X - num370;
                Main.dust[num372].position.Y = projectile.Center.Y - num371;
                Dust dust = Main.dust[num372];
                dust.velocity *= 0f;
                Main.dust[num372].scale = 0.5f;
                num3 = num369;
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) - 1.57f;
            float num373 = projectile.position.X;
            float num374 = projectile.position.Y;
            float num375 = 100000f;
            bool flag10 = false;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 30f)
            {
                projectile.ai[0] = 30f;
                int num4;
                for (int num376 = 0; num376 < 200; num376 = num4 + 1)
                {
                    if (Main.npc[num376].CanBeChasedBy(projectile, false) && (!Main.npc[num376].wet || projectile.type == 307))
                    {
                        float num377 = Main.npc[num376].position.X + (float)(Main.npc[num376].width / 2);
                        float num378 = Main.npc[num376].position.Y + (float)(Main.npc[num376].height / 2);
                        float num379 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num377) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num378);
                        if (num379 < 800f && num379 < num375 && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[num376].position, Main.npc[num376].width, Main.npc[num376].height))
                        {
                            num375 = num379;
                            num373 = num377;
                            num374 = num378;
                            flag10 = true;
                        }
                    }
                    num4 = num376;
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
                    return;
                }
            }
            else if (projectile.velocity.Y > num383)
            {
                projectile.velocity.Y = projectile.velocity.Y - num381;
                if (projectile.velocity.Y > 0f && num383 < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num381 * 2f;
                    return;
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
    }
}