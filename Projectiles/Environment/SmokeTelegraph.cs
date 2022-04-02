using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Environment
{
    public class SmokeTelegraph : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dustType = 31;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Smoke");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.trap = true;
        }

        public override void AI()
        {
            int num1 = Math.Sign(projectile.velocity.Y);
            int num2 = num1 == -1 ? 0 : 1;
            if (projectile.ai[0] == 0f)
            {
                if (!Collision.SolidCollision(projectile.position + new Vector2(0f, num1 == -1 ? (float) (projectile.height - 48) : 0f), projectile.width, 48) && !Collision.WetCollision(projectile.position + new Vector2(0f, num1 == -1 ? (float) (projectile.height - 20) : 0f), projectile.width, 20))
                {
                    projectile.velocity = new Vector2(0f, (float) Math.Sign(projectile.velocity.Y) * (1f / 1000f));
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.timeLeft = 60;
                }
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 60f)
                    projectile.Kill();
                for (int index1 = 0; index1 < 3; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, new Color(), 1f);
                    Main.dust[index2].scale = 0.1f + Main.rand.Next(5) * 0.1f;
                    Main.dust[index2].fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = projectile.Center + new Vector2(0f, (float) (-projectile.height / 2)).RotatedBy((double)projectile.rotation, new Vector2()) * 1.1f;
                }
            }
            if (projectile.ai[0] != 1f)
                return;
            projectile.velocity = new Vector2(0f, (float) Math.Sign(projectile.velocity.Y) * (1f / 1000f));
            if (num1 != 0)
            {
                int num3 = 16;
                int num4 = 320;
                while (num3 < num4 && !Collision.SolidCollision(projectile.position + new Vector2(0f, num1 == -1 ? (float)(projectile.height - num3 - 16) : 0f), projectile.width, num3 + 16))
                    num3 += 16;
                if (num1 == -1)
                {
                    projectile.position.Y += (float)projectile.height;
                    projectile.height = num3;
                    projectile.position.Y -= (float)num3;
                }
                else
                    projectile.height = num3;
            }
            projectile.ai[1] += 1f;
            if (projectile.ai[1] >= 60f)
                projectile.Kill();
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                for (int index1 = 0; index1 < 60; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, -2.5f * (float) -num1, 0, new Color(), 1f);
                    Dust dust = Main.dust[index2];
                    dust.alpha = 200;
                    dust.velocity *= new Vector2(0.3f, 2f);
                    dust.velocity.Y += (float)(2 * num1);
                    dust.scale += Main.rand.NextFloat();
                    dust.position = new Vector2(projectile.Center.X, projectile.Center.Y + (float) projectile.height * 0.5f * (float) -num1);
                    dust.customData = (object) num2;
                    if (num1 == -1 && Main.rand.Next(4) != 0)
                    {
                        dust.velocity.Y -= 0.2f;
                    }
                }
                Main.PlaySound(SoundID.Item34, projectile.position);
            }
            if (num1 == 1)
            {
                for (int index1 = 0; index1 < 9; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, -2.5f * (float) -num1, 0, new Color(), 1f);
                    Dust dust = Main.dust[index2];
                    dust.alpha = 200;
                    dust.velocity *= new Vector2(0.3f, 2f);
                    dust.velocity.Y += (float)(2 * num1);
                    dust.scale += Main.rand.NextFloat();
                    dust.position = new Vector2(projectile.Center.X, projectile.Center.Y + (float) projectile.height * 0.5f * (float) -num1);
                    dust.customData = (object)num2;
                    if (num1 == -1 && Main.rand.Next(4) != 0)
                    {
                        Main.dust[index2].velocity.Y -= 0.2f;
                    }
                }
            }
            int Height = (int)(projectile.ai[1] / 60f * (float)projectile.height) * 3;
            if (Height > projectile.height)
                Height = projectile.height;
            Vector2 Position = projectile.position + (num1 == -1 ? new Vector2(0f, (float) (projectile.height - Height)) : Vector2.Zero);
            Vector2 vector2 = projectile.position + (num1 == -1 ? new Vector2(0f, (float) projectile.height) : Vector2.Zero);
            for (int index1 = 0; index1 < 6; ++index1)
            {
                if (Main.rand.Next(3) < 2)
                {
                    int index2 = Dust.NewDust(Position, projectile.width, Height, dustType, 0f, 0f, 90, new Color(), 2.5f);
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
                        dust.position.X = projectile.Center.X;
                        dust.noGravity = false;
                        dust.noLight = true;
                        dust.fadeIn = 0.4f;
                        dust.scale *= 0.3f;
                    }
                    else
                        Main.dust[index2].velocity = projectile.DirectionFrom(Main.dust[index2].position) * Main.dust[index2].velocity.Length() * 0.25f;
                    Main.dust[index2].velocity.Y *= (float)-num1;
                    Main.dust[index2].customData = (object)num2;
                }
            }
            for (int index1 = 0; index1 < 6; ++index1)
            {
                if (Main.rand.NextFloat() >= 0.5f)
                {
                    int index2 = Dust.NewDust(Position, projectile.width, Height, dustType, 0f, -2.5f * (float) -num1, 0, new Color(), 1f);
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

        public override bool CanDamage() => false;
    }
}
