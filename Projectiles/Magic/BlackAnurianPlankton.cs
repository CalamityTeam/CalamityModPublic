using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BlackAnurianPlankton : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plankton");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 50;
            }
            else
            {
                Projectile.extraUpdates = 0;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            float num373 = Projectile.position.X;
            float num374 = Projectile.position.Y;
            float num375 = 100000f;
            bool flag10 = false;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 30f)
            {
                Projectile.ai[0] = 30f;
                for (int num376 = 0; num376 < Main.maxNPCs; num376++)
                {
                    if (Main.npc[num376].CanBeChasedBy(Projectile, false) && !Main.npc[num376].wet)
                    {
                        float num377 = Main.npc[num376].position.X + (float)(Main.npc[num376].width / 2);
                        float num378 = Main.npc[num376].position.Y + (float)(Main.npc[num376].height / 2);
                        float num379 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num377) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num378);
                        if (num379 < 800f && num379 < num375 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num376].position, Main.npc[num376].width, Main.npc[num376].height))
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
                num373 = Projectile.position.X + (float)(Projectile.width / 2) + Projectile.velocity.X * 100f;
                num374 = Projectile.position.Y + (float)(Projectile.height / 2) + Projectile.velocity.Y * 100f;
            }

            float num380 = 6f;
            float num381 = 0.1f;
            Vector2 vector30 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float num382 = num373 - vector30.X;
            float num383 = num374 - vector30.Y;
            float num384 = (float)Math.Sqrt((double)(num382 * num382 + num383 * num383));
            num384 = num380 / num384;
            num382 *= num384;
            num383 *= num384;
            if (Projectile.velocity.X < num382)
            {
                Projectile.velocity.X = Projectile.velocity.X + num381;
                if (Projectile.velocity.X < 0f && num382 > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + num381 * 2f;
                }
            }
            else if (Projectile.velocity.X > num382)
            {
                Projectile.velocity.X = Projectile.velocity.X - num381;
                if (Projectile.velocity.X > 0f && num382 < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - num381 * 2f;
                }
            }
            if (Projectile.velocity.Y < num383)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + num381;
                if (Projectile.velocity.Y < 0f && num383 > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + num381 * 2f;
                }
            }
            else if (Projectile.velocity.Y > num383)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - num381;
                if (Projectile.velocity.Y > 0f && num383 < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - num381 * 2f;
                }
            }
        }
    }
}
