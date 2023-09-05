using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ScourgeoftheCosmosMini : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private int bounce = 3;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 375;
            Projectile.extraUpdates = 4;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 270 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 50;
            else
                Projectile.extraUpdates = 1;

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 1)
                Projectile.frame = 0;

            for (int num369 = 0; num369 < 1; num369++)
            {
                int dustType = Main.rand.NextBool(3) ? 56 : 242;
                float dustX = Projectile.velocity.X / 3f * num369;
                float dustY = Projectile.velocity.Y / 3f * num369;
                int num372 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[num372];
                dust.position.X = Projectile.Center.X - dustX;
                dust.position.Y = Projectile.Center.Y - dustY;
                dust.velocity *= 0f;
                dust.scale = 0.5f;
            }

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathHelper.PiOver2;

            float num373 = Projectile.position.X;
            float num374 = Projectile.position.Y;
            float num375 = 100000f;
            bool isHoming = false;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 30f)
            {
                Projectile.ai[0] = 30f;
                for (int enemy = 0; enemy < Main.maxNPCs; enemy++)
                {
                    if (Main.npc[enemy].CanBeChasedBy(Projectile, false))
                    {
                        float enemyX = Main.npc[enemy].position.X + Main.npc[enemy].width / 2;
                        float enemyY = Main.npc[enemy].position.Y + Main.npc[enemy].height / 2;
                        float enemyDistance = Math.Abs(Projectile.position.X + Projectile.width / 2 - enemyX) + Math.Abs(Projectile.position.Y + Projectile.height / 2 - enemyY);
                        if (enemyDistance < 800f && enemyDistance < num375 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[enemy].position, Main.npc[enemy].width, Main.npc[enemy].height))
                        {
                            num375 = enemyDistance;
                            num373 = enemyX;
                            num374 = enemyY;
                            isHoming = true;
                        }
                    }
                }
            }
            if (!isHoming)
            {
                num373 = Projectile.position.X + Projectile.width / 2 + Projectile.velocity.X * 100f;
                num374 = Projectile.position.Y + Projectile.height / 2 + Projectile.velocity.Y * 100f;
            }

            float num381 = 0.16f;
            Vector2 vector30 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
            float num382 = num373 - vector30.X;
            float num383 = num374 - vector30.Y;
            float num384 = (float)Math.Sqrt(num382 * num382 + num383 * num383);
            num384 = 10f / num384;
            num382 *= num384;
            num383 *= num384;
            if (Projectile.velocity.X < num382)
            {
                Projectile.velocity.X = Projectile.velocity.X + num381;
                if (Projectile.velocity.X < 0f && num382 > 0f)
                    Projectile.velocity.X = Projectile.velocity.X + num381 * 2f;
            }
            else if (Projectile.velocity.X > num382)
            {
                Projectile.velocity.X = Projectile.velocity.X - num381;
                if (Projectile.velocity.X > 0f && num382 < 0f)
                    Projectile.velocity.X = Projectile.velocity.X - num381 * 2f;
            }
            if (Projectile.velocity.Y < num383)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + num381;
                if (Projectile.velocity.Y < 0f && num383 > 0f)
                    Projectile.velocity.Y = Projectile.velocity.Y + num381 * 2f;
            }
            else if (Projectile.velocity.Y > num383)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - num381;
                if (Projectile.velocity.Y > 0f && num383 < 0f)
                    Projectile.velocity.Y = Projectile.velocity.Y - num381 * 2f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Vector2 origin = new Vector2(9f, 10f);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/ScourgeoftheCosmosMiniGlow").Value, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}
