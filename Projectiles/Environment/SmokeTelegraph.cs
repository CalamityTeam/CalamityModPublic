using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Environment
{
    public class SmokeTelegraph : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dustType = 31;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.trap = true;
        }

        public override void AI()
        {
            int num1 = Math.Sign(Projectile.velocity.Y);
            int num2 = num1 == -1 ? 0 : 1;
            if (Projectile.ai[0] == 0f)
            {
                if (!Collision.SolidCollision(Projectile.position + new Vector2(0f, num1 == -1 ? (float) (Projectile.height - 48) : 0f), Projectile.width, 48) && !Collision.WetCollision(Projectile.position + new Vector2(0f, num1 == -1 ? (float) (Projectile.height - 20) : 0f), Projectile.width, 20))
                {
                    Projectile.velocity = new Vector2(0f, (float) Math.Sign(Projectile.velocity.Y) * (1f / 1000f));
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.timeLeft = 60;
                }

                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= 60f)
                    Projectile.Kill();

                for (int index1 = 0; index1 < 3; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, new Color(), 1f);
                    Main.dust[index2].scale = 0.1f + Main.rand.Next(5) * 0.1f;
                    Main.dust[index2].fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = Projectile.Center + new Vector2(0f, (float) (-Projectile.height / 2)).RotatedBy((double)Projectile.rotation, new Vector2()) * 1.1f;
                }
            }

            if (Projectile.ai[0] != 1f)
                return;

            Projectile.velocity = new Vector2(0f, (float) Math.Sign(Projectile.velocity.Y) * (1f / 1000f));

            if (num1 != 0)
            {
                int num3 = 16;
                int num4 = 320;
                while (num3 < num4 && !Collision.SolidCollision(Projectile.position + new Vector2(0f, num1 == -1 ? (float)(Projectile.height - num3 - 16) : 0f), Projectile.width, num3 + 16))
                    num3 += 16;

                if (num1 == -1)
                {
                    Projectile.position.Y += (float)Projectile.height;
                    Projectile.height = num3;
                    Projectile.position.Y -= (float)num3;
                }
                else
                    Projectile.height = num3;
            }

            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] >= 60f)
                Projectile.Kill();

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                for (int index1 = 0; index1 < 60; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, -2.5f * (float) -num1, 0, new Color(), 1f);
                    Dust dust = Main.dust[index2];
                    dust.alpha = 200;
                    dust.velocity *= new Vector2(0.3f, 2f);
                    dust.velocity.Y += (float)(2 * num1);
                    dust.scale += Main.rand.NextFloat();
                    dust.position = new Vector2(Projectile.Center.X, Projectile.Center.Y + (float) Projectile.height * 0.5f * (float) -num1);
                    dust.customData = (object) num2;
                    if (num1 == -1 && Main.rand.Next(4) != 0)
                    {
                        dust.velocity.Y -= 0.2f;
                    }
                }
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
            }
            if (num1 == 1)
            {
                for (int index1 = 0; index1 < 9; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, -2.5f * (float) -num1, 0, new Color(), 1f);
                    Dust dust = Main.dust[index2];
                    dust.alpha = 200;
                    dust.velocity *= new Vector2(0.3f, 2f);
                    dust.velocity.Y += (float)(2 * num1);
                    dust.scale += Main.rand.NextFloat();
                    dust.position = new Vector2(Projectile.Center.X, Projectile.Center.Y + (float) Projectile.height * 0.5f * (float) -num1);
                    dust.customData = (object)num2;
                    if (num1 == -1 && Main.rand.Next(4) != 0)
                    {
                        Main.dust[index2].velocity.Y -= 0.2f;
                    }
                }
            }
            int Height = (int)(Projectile.ai[1] / 60f * (float)Projectile.height) * 3;
            if (Height > Projectile.height)
                Height = Projectile.height;
            Vector2 Position = Projectile.position + (num1 == -1 ? new Vector2(0f, (float) (Projectile.height - Height)) : Vector2.Zero);
            Vector2 vector2 = Projectile.position + (num1 == -1 ? new Vector2(0f, (float) Projectile.height) : Vector2.Zero);
            for (int index1 = 0; index1 < 6; ++index1)
            {
                if (Main.rand.Next(3) < 2)
                {
                    int index2 = Dust.NewDust(Position, Projectile.width, Height, dustType, 0f, 0f, 90, new Color(), 2.5f);
                    Dust dust = Main.dust[index2];
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    if (dust.velocity.Y > 0f)
                    {
                        dust.velocity.Y *= -1f;
                    }
                    if (Main.rand.Next(6) < 3)
                    {
                        dust.position.Y = MathHelper.Lerp(dust.position.Y, vector2.Y, 0.5f);
                        dust.velocity *= 5f;
                        dust.velocity.Y -= 3f;
                        dust.position.X = Projectile.Center.X;
                        dust.noGravity = false;
                        dust.noLight = true;
                        dust.fadeIn = 0.4f;
                        dust.scale *= 0.3f;
                    }
                    else
                        Main.dust[index2].velocity = Projectile.DirectionFrom(Main.dust[index2].position) * Main.dust[index2].velocity.Length() * 0.25f;
                    Main.dust[index2].velocity.Y *= (float)-num1;
                    Main.dust[index2].customData = (object)num2;
                }
            }
            for (int index1 = 0; index1 < 6; ++index1)
            {
                if (Main.rand.NextFloat() >= 0.5f)
                {
                    int index2 = Dust.NewDust(Position, Projectile.width, Height, dustType, 0f, -2.5f * (float) -num1, 0, new Color(), 1f);
                    Dust dust = Main.dust[index2];
                    dust.alpha = 200;
                    dust.velocity *= new Vector2(0.6f, 1.5f);
                    dust.scale += Main.rand.NextFloat();
                    if (num1 == -1 && Main.rand.Next(4) != 0)
                    {
                        dust.velocity.Y -= 0.2f;
                    }
                    dust.customData = (object)num2;
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
