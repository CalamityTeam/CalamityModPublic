using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class DarkSparkBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/YharimsCrystalBeam";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Vector2? vector71 = null;

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            if (Projectile.type != ModContent.ProjectileType<DarkSparkBeam>() || !Main.projectile[(int)Projectile.ai[1]].active || Main.projectile[(int)Projectile.ai[1]].type != ModContent.ProjectileType<DarkSparkPrism>())
            {
                Projectile.Kill();
                return;
            }

            float laserPosition = (int)Projectile.ai[0] - 2.5f;
            Vector2 value36 = Vector2.Normalize(Main.projectile[(int)Projectile.ai[1]].velocity);
            Projectile projectile2 = Main.projectile[(int)Projectile.ai[1]];
            float num811;
            Vector2 value37 = Vector2.Zero;
            float num812;
            float y;
            float laserRotationSpeed;
            float scaleFactor6;
            Color color = new Color(1, 1, 1, 127);
            Projectile.Opacity = 1f;

            if (projectile2.ai[0] < 720f)
            {
                num812 = projectile2.ai[0] / 1440f;
                y = 6f + projectile2.ai[0] / 720f * 7f;
                if (projectile2.ai[0] > 360f)
                {
                    int colorValue = (int)((0.01f + ((projectile2.ai[0] - 360f) / 360f * 2.55f)) * 100f);
                    color = new Color(colorValue, colorValue, colorValue, 127);
                }

                if (projectile2.ai[0] < 480f)
                    laserRotationSpeed = 1.75f;
                else
                    laserRotationSpeed = 3f + 5f * ((projectile2.ai[0] - 480f) / 240f);

                scaleFactor6 = -2f - projectile2.ai[0] / 720f * 5f;
            }
            else
            {
                float colorLimit = projectile2.ai[0] - 720f;
                if (colorLimit > 255f)
                    colorLimit = 255f;

                switch ((int)Projectile.ai[0]) //R O Y G B I V
                {
                    case 0:
                        color = new Color(255, 255 - (int)colorLimit, 255 - (int)colorLimit, 127);
                        break;
                    case 1:
                        color = new Color(255, 255 - (int)(colorLimit * 0.3529412f), 255 - (int)colorLimit, 127);
                        break;
                    case 2:
                        color = new Color(255, 255, 255 - (int)colorLimit, 127);
                        break;
                    case 3:
                        color = new Color(255 - (int)colorLimit, 255 - (int)(colorLimit * 0.5f), 255 - (int)colorLimit, 127);
                        break;
                    case 4:
                        color = new Color(255 - (int)colorLimit, 255 - (int)colorLimit, 255, 127);
                        break;
                    case 5:
                        color = new Color(255 - (int)(colorLimit * 0.7058824f), 255 - (int)colorLimit, 255 - (int)(colorLimit * 0.4901961), 127);
                        break;
                    case 6:
                        color = new Color(255 - (int)(colorLimit * 0.0666667f), 255 - (int)(colorLimit * 0.4901961), 255 - (int)(colorLimit * 0.0666667f), 127);
                        break;
                }

                num812 = 0.5f;
                laserRotationSpeed = 10.875f;
                y = 13f;
                scaleFactor6 = -7f;
            }

            float num814 = (projectile2.ai[0] + laserPosition * laserRotationSpeed) / (laserRotationSpeed * 6f) * MathHelper.TwoPi;
            num811 = Vector2.UnitY.RotatedBy(num814).Y * (MathHelper.Pi / 6) * num812 * 0.33f;
            value37 = (Vector2.UnitY.RotatedBy(num814) * new Vector2(4f, y)).RotatedBy(projectile2.velocity.ToRotation());
            Projectile.position = projectile2.Center + value36 * 16f - Projectile.Size / 2f + new Vector2(0f, -Main.projectile[(int)Projectile.ai[1]].gfxOffY);
            Projectile.position += projectile2.velocity.ToRotation().ToRotationVector2() * scaleFactor6;
            Projectile.position += value37;
            Projectile.velocity = Vector2.Normalize(projectile2.velocity).RotatedBy(num811);
            Projectile.scale = 1.5f * (1.5f - num812);

            // Takes 360 frames to reach normal damage
            float amount = projectile2.ai[0] / 1200f;
            if (amount > 1f)
                amount = 1f;
            Projectile.damage = (int)(projectile2.damage * MathHelper.Lerp(0.25f, 2.2f, amount));

            if (projectile2.ai[0] >= 720f)
                vector71 = new Vector2?(projectile2.Center);

            if (!Collision.CanHitLine(Main.player[Projectile.owner].Center, 0, 0, projectile2.Center, 0, 0))
                vector71 = new Vector2?(Main.player[Projectile.owner].Center);

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            float num818 = Projectile.velocity.ToRotation();
            Projectile.rotation = num818 - MathHelper.PiOver2;
            Projectile.velocity = num818.ToRotationVector2();

            float num819 = 2f;
            float num820 = 0f;
            Vector2 samplingPoint = Projectile.Center;
            if (vector71.HasValue)
                samplingPoint = vector71.Value;

            float[] array3 = new float[(int)num819];
            Collision.LaserScan(samplingPoint, Projectile.velocity, num820 * Projectile.scale, 2400f, array3);
            float num821 = 0f;
            for (int num822 = 0; num822 < array3.Length; num822++)
                num821 += array3[num822];
            num821 /= num819;

            float amount2 = 0.75f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num821, amount2);
            if (Math.Abs(Projectile.localAI[1] - num821) < 100f && Projectile.scale > 0.15f)
            {
                Vector2 vector80 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14.5f * Projectile.scale);
                for (int num843 = 0; num843 < 2; num843++)
                {
                    float num844 = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                    float num845 = (float)Main.rand.NextDouble() * 0.8f + 1f;
                    Vector2 vector81 = new Vector2((float)Math.Cos(num844) * num845, (float)Math.Sin(num844) * num845);
                    int num846 = Dust.NewDust(vector80, 0, 0, 267, vector81.X, vector81.Y, 0, color, 3.3f);
                    Main.dust[num846].color = color;
                    Main.dust[num846].scale = 1.2f;
                    if (Projectile.scale > 1f)
                    {
                        Main.dust[num846].velocity *= Projectile.scale;
                        Main.dust[num846].scale *= Projectile.scale;
                    }
                    Main.dust[num846].noGravity = true;
                    if (Projectile.scale != 1.4f)
                    {
                        Dust dust9 = Dust.CloneDust(num846);
                        dust9.color = color;
                        dust9.scale /= 2f;
                        dust9.noGravity = true;
                    }
                    Main.dust[num846].color = color;
                }

                if (Main.rand.NextBool(5))
                {
                    Vector2 value42 = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                    int num847 = Dust.NewDust(vector80 + value42 - Vector2.One * 4f, 8, 8, 267, 0f, 0f, 100, color, 5f);
                    Main.dust[num847].velocity *= 0.5f;
                    Main.dust[num847].noGravity = true;
                    Main.dust[num847].velocity.Y = -Math.Abs(Main.dust[num847].velocity.Y);
                }

                DelegateMethods.v3_1 = color.ToVector3() * 0.3f;
                float value43 = 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
                Vector2 size = new Vector2(Projectile.velocity.Length() * Projectile.localAI[1], Projectile.width * Projectile.scale);
                float num848 = Projectile.velocity.ToRotation();
                if (Main.netMode != NetmodeID.Server)
                    ((WaterShaderData)Filters.Scene["WaterDistortion"].GetShader()).QueueRipple(Projectile.position + new Vector2(size.X * 0.5f, 0f).RotatedBy(num848), color, size, RippleShape.Square, num848);

                Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            float num228 = Projectile.localAI[1];
            Projectile projectile2 = Main.projectile[(int)Projectile.ai[1]];
            Color color = new Color(1, 1, 1, 127);
            if (projectile2.ai[0] < 720f)
            {
                if (projectile2.ai[0] > 360f)
                {
                    int colorValue = (int)((0.01f + ((projectile2.ai[0] - 360f) / 360f * 2.55f)) * 100f);
                    color = new Color(colorValue, colorValue, colorValue, 127);
                }
            }
            else
            {
                float colorLimit = projectile2.ai[0] - 720f;
                if (colorLimit > 255f)
                {
                    colorLimit = 255f;
                }
                switch ((int)Projectile.ai[0]) //R O Y G B I V
                {
                    case 0:
                        color = new Color(255, 255 - (int)colorLimit, 255 - (int)colorLimit, 127);
                        break;
                    case 1:
                        color = new Color(255, 255 - (int)(colorLimit * 0.3529412f), 255 - (int)colorLimit, 127);
                        break;
                    case 2:
                        color = new Color(255, 255, 255 - (int)colorLimit, 127);
                        break;
                    case 3:
                        color = new Color(255 - (int)colorLimit, 255 - (int)(colorLimit * 0.5f), 255 - (int)colorLimit, 127);
                        break;
                    case 4:
                        color = new Color(255 - (int)colorLimit, 255 - (int)colorLimit, 255, 127);
                        break;
                    case 5:
                        color = new Color(255 - (int)(colorLimit * 0.7058824f), 255 - (int)colorLimit, 255 - (int)(colorLimit * 0.4901961), 127);
                        break;
                    case 6:
                        color = new Color(255 - (int)(colorLimit * 0.0666667f), 255 - (int)(colorLimit * 0.4901961), 255 - (int)(colorLimit * 0.0666667f), 127);
                        break;
                }
            }

            Color value25 = color;
            Vector2 value26 = Projectile.Center.Floor();
            value26 += Projectile.velocity * Projectile.scale * 10.5f;
            num228 -= Projectile.scale * 14.5f * Projectile.scale;
            Vector2 vector29 = new Vector2(Projectile.scale);
            DelegateMethods.f_1 = 1f;
            DelegateMethods.c_1 = value25 * 0.75f * Projectile.Opacity;
            Vector2 projPos = Projectile.oldPos[0];
            projPos = new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Utils.DrawLaser(Main.spriteBatch, tex, value26 - Main.screenPosition, value26 + Projectile.velocity * num228 - Main.screenPosition, vector29, new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));
            DelegateMethods.c_1 = color * 0.75f * Projectile.Opacity;
            Utils.DrawLaser(Main.spriteBatch, tex, value26 - Main.screenPosition, value26 + Projectile.velocity * num228 - Main.screenPosition, vector29 / 2f, new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref num6))
                return true;

            return false;
        }
    }
}
