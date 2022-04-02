using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ScourgeoftheCosmosMini : ModProjectile
    {
        private int bounce = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miniature Cosmic Scourge");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.melee = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 3;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 270 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            if (projectile.alpha > 0)
                projectile.alpha -= 50;
            else
                projectile.extraUpdates = 1;

            if (projectile.alpha < 0)
                projectile.alpha = 0;

            projectile.frameCounter++;
            if (projectile.frameCounter >= 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 1)
                projectile.frame = 0;

            for (int num369 = 0; num369 < 1; num369++)
            {
                int dustType = Main.rand.NextBool(3) ? 56 : 242;
                float num370 = projectile.velocity.X / 3f * num369;
                float num371 = projectile.velocity.Y / 3f * num369;
                int num372 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[num372];
                dust.position.X = projectile.Center.X - num370;
                dust.position.Y = projectile.Center.Y - num371;
                dust.velocity *= 0f;
                dust.scale = 0.5f;
            }

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - MathHelper.PiOver2;

            float num373 = projectile.position.X;
            float num374 = projectile.position.Y;
            float num375 = 100000f;
            bool flag10 = false;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 30f)
            {
                projectile.ai[0] = 30f;
                for (int num376 = 0; num376 < Main.maxNPCs; num376++)
                {
                    if (Main.npc[num376].CanBeChasedBy(projectile, false))
                    {
                        float num377 = Main.npc[num376].position.X + Main.npc[num376].width / 2;
                        float num378 = Main.npc[num376].position.Y + Main.npc[num376].height / 2;
                        float num379 = Math.Abs(projectile.position.X + projectile.width / 2 - num377) + Math.Abs(projectile.position.Y + projectile.height / 2 - num378);
                        if (num379 < 800f && num379 < num375 && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[num376].position, Main.npc[num376].width, Main.npc[num376].height))
                        {
                            num375 = num379;
                            num373 = num377;
                            num374 = num378;
                            flag10 = true;
                        }
                    }
                }
            }
            if (!flag10)
            {
                num373 = projectile.position.X + projectile.width / 2 + projectile.velocity.X * 100f;
                num374 = projectile.position.Y + projectile.height / 2 + projectile.velocity.Y * 100f;
            }

            float num380 = 10f;
            float num381 = 0.16f;
            Vector2 vector30 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
            float num382 = num373 - vector30.X;
            float num383 = num374 - vector30.Y;
            float num384 = (float)Math.Sqrt(num382 * num382 + num383 * num383);
            num384 = num380 / num384;
            num382 *= num384;
            num383 *= num384;
            if (projectile.velocity.X < num382)
            {
                projectile.velocity.X = projectile.velocity.X + num381;
                if (projectile.velocity.X < 0f && num382 > 0f)
                    projectile.velocity.X = projectile.velocity.X + num381 * 2f;
            }
            else if (projectile.velocity.X > num382)
            {
                projectile.velocity.X = projectile.velocity.X - num381;
                if (projectile.velocity.X > 0f && num382 < 0f)
                    projectile.velocity.X = projectile.velocity.X - num381 * 2f;
            }
            if (projectile.velocity.Y < num383)
            {
                projectile.velocity.Y = projectile.velocity.Y + num381;
                if (projectile.velocity.Y < 0f && num383 > 0f)
                    projectile.velocity.Y = projectile.velocity.Y + num381 * 2f;
            }
            else if (projectile.velocity.Y > num383)
            {
                projectile.velocity.Y = projectile.velocity.Y - num381;
                if (projectile.velocity.Y > 0f && num383 < 0f)
                    projectile.velocity.Y = projectile.velocity.Y - num381 * 2f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
                projectile.Kill();
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                    projectile.velocity.X = -oldVelocity.X;
                if (projectile.velocity.Y != oldVelocity.Y)
                    projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Vector2 origin = new Vector2(9f, 10f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Melee/ScourgeoftheCosmosMiniGlow"), projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Color.White, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
