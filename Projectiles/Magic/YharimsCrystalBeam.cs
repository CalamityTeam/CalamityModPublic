using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace CalamityMod.Projectiles.Magic
{
    public class YharimsCrystalBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[1] = reader.ReadSingle();
        }

        public float GetHue(float indexing)
        {
            string playerName;
            if (Main.player[projectile.owner].active && (playerName = Main.player[projectile.owner].name) != null)
            {
                Dictionary<string, int> LaserHue = new Dictionary<string, int>(17)
                {
                    {"Fabsol", 0},
                    {"Ziggums", 1},
                    {"Poly", 2},
                    {"Zach", 3},
                    {"Grox the Great", 4},
                    {"Jenosis", 5},
                    {"DM DOKURO", 6},
                    {"Uncle Danny", 7},
                    {"Phoenix", 8},
                    {"MineCat", 9},
                    {"Khaelis", 10},
                    {"Purple Necromancer", 11},
                    {"gamagamer64", 12},
                    {"Svante", 13},
                    {"Puff", 14},
                    {"Leviathan", 15},
                    {"Testdude", 16}
                };
                if (LaserHue.TryGetValue(playerName, out int someNumber))
                {
                    switch (someNumber)
                    {
                        case 0:
                        case 1:
                            return 2f;
                        case 2:
                            return 0.83f;
                        case 3:
                            return 1.5f + (float)Math.Cos(Main.time / 180.0 * 6.2831854820251465) * 0.1f;
                        case 4:
                            return 1.27f;
                        case 5:
                            return 0.65f + (float)Math.Cos(Main.time / 180.0 * 6.2831854820251465) * 0.1f;
                        case 6:
                            return 0f;
                        case 7:
                        case 8:
                            return 1.7f + (float)Math.Cos(Main.time / 180.0 * 6.2831854820251465) * 0.07f;
                        case 9:
                            return 0.15f + (float)Math.Cos(Main.time / 180.0 * 6.2831854820251465) * 0.07f;
                        case 10:
                            return 1.15f + (float)Math.Cos(Main.time / 180.0 * 6.2831854820251465) * 0.18f;
                        case 11:
                            return 1.7f + (float)Math.Cos(Main.time / 120.0 * 6.2831854820251465) * 0.05f;
                        case 12:
                            return 0.83f + (float)Math.Cos(Main.time / 120.0 * 6.2831854820251465) * 0.03f;
                        case 13:
                            return 1.4f + (float)Math.Cos(Main.time / 180.0 * 6.2831854820251465) * 0.06f;
                        case 14:
                            return 0.31f + (float)Math.Cos(Main.time / 120.0 * 6.2831854820251465) * 0.13f;
                        case 15:
                            return 1.9f + (float)Math.Cos(Main.time / 180.0 * 6.2831854820251465) * 0.1f;
                        case 16:
                            return Main.rand.NextFloat();
                    }
                }
            }
            return (float)(int)indexing / 6f;
        }

        public override void AI()
        {
            Vector2? vector71 = null;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            if (projectile.type != ModContent.ProjectileType<YharimsCrystalBeam>() || !Main.projectile[(int)projectile.ai[1]].active || Main.projectile[(int)projectile.ai[1]].type != ModContent.ProjectileType<YharimsCrystal>())
            {
                projectile.Kill();
                return;
            }
            float num810 = (float)(int)projectile.ai[0] - 2.5f;
            Vector2 value36 = Vector2.Normalize(Main.projectile[(int)projectile.ai[1]].velocity);
            Projectile projectile2 = Main.projectile[(int)projectile.ai[1]];
            float num811 = num810 * 0.5235988f;
            Vector2 value37 = Vector2.Zero;
            float num812;
            float y;
            float num813;
            float scaleFactor6;
            if (projectile2.ai[0] < 180f)
            {
                num812 = 1f - projectile2.ai[0] / 180f;
                y = 20f - projectile2.ai[0] / 180f * 14f;
                if (projectile2.ai[0] < 120f)
                {
                    num813 = 20f - 4f * (projectile2.ai[0] / 120f);
                    projectile.Opacity = projectile2.ai[0] / 120f * 0.4f;
                }
                else
                {
                    num813 = 16f - 10f * ((projectile2.ai[0] - 120f) / 60f);
                    projectile.Opacity = 0.4f + (projectile2.ai[0] - 120f) / 60f * 0.6f;
                }
                scaleFactor6 = -22f + projectile2.ai[0] / 180f * 20f;
            }
            else
            {
                num812 = 0f;
                num813 = 1.75f;
                y = 6f;
                projectile.Opacity = 1f;
                scaleFactor6 = -2f;
            }
            float num814 = (projectile2.ai[0] + num810 * num813) / (num813 * 6f) * 6.28318548f;
            num811 = Vector2.UnitY.RotatedBy((double)num814, default).Y * 0.5235988f * num812;
            value37 = (Vector2.UnitY.RotatedBy((double)num814, default) * new Vector2(4f, y)).RotatedBy((double)projectile2.velocity.ToRotation(), default);
            projectile.position = projectile2.Center + value36 * 16f - projectile.Size / 2f + new Vector2(0f, -Main.projectile[(int)projectile.ai[1]].gfxOffY);
            projectile.position += projectile2.velocity.ToRotation().ToRotationVector2() * scaleFactor6;
            projectile.position += value37;
            projectile.velocity = Vector2.Normalize(projectile2.velocity).RotatedBy((double)num811, default);
            projectile.scale = 1.8f * (1f - num812);
            projectile.damage = projectile2.damage;
            if (projectile2.ai[0] >= 180f)
            {
                projectile.damage = (int)((double)projectile.damage * 2.7);
                vector71 = new Vector2?(projectile2.Center);
            }
            if (!Collision.CanHitLine(Main.player[projectile.owner].Center, 0, 0, projectile2.Center, 0, 0))
            {
                vector71 = new Vector2?(Main.player[projectile.owner].Center);
            }
            projectile.friendly = projectile2.ai[0] > 30f;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            float num818 = projectile.velocity.ToRotation();
            projectile.rotation = num818 - 1.57079637f;
            projectile.velocity = num818.ToRotationVector2();
            float num819 = 2f;
            float num820 = 0f;
            Vector2 samplingPoint = projectile.Center;
            if (vector71.HasValue)
            {
                samplingPoint = vector71.Value;
            }
            float[] array3 = new float[(int)num819];
            Collision.LaserScan(samplingPoint, projectile.velocity, num820 * projectile.scale, 2400f, array3);
            float num821 = 0f;
            for (int num822 = 0; num822 < array3.Length; num822++)
            {
                num821 += array3[num822];
            }
            num821 /= num819;
            float amount = 0.75f;
            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num821, amount);
            if (Math.Abs(projectile.localAI[1] - num821) < 100f && projectile.scale > 0.15f)
            {
                float prismHue = GetHue(projectile.ai[0]);
                Color color = Main.hslToRgb(2.55f, prismHue, 0.53f);
                color.A = 0;
                Vector2 vector80 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14.5f * projectile.scale);
                float x = Main.rgbToHsl(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB)).X;
                for (int num843 = 0; num843 < 2; num843++)
                {
                    float num844 = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                    float num845 = (float)Main.rand.NextDouble() * 0.8f + 1f;
                    Vector2 vector81 = new Vector2((float)Math.Cos((double)num844) * num845, (float)Math.Sin((double)num844) * num845);
                    int num846 = Dust.NewDust(vector80, 0, 0, 244, vector81.X, vector81.Y, 0, new Color(255, Main.DiscoG, 53), 3.3f); //267
                    Main.dust[num846].color = color;
                    Main.dust[num846].scale = 1.2f;
                    if (projectile.scale > 1f)
                    {
                        Main.dust[num846].velocity *= projectile.scale;
                        Main.dust[num846].scale *= projectile.scale;
                    }
                    Main.dust[num846].noGravity = true;
                    if (projectile.scale != 1.4f)
                    {
                        Dust dust9 = Dust.CloneDust(num846);
                        dust9.color = Color.Orange;
                        dust9.scale /= 2f;
                    }
                    float hue = (x + Main.rand.NextFloat() * 0.4f) % 1f;
                    Main.dust[num846].color = Color.Lerp(color, Main.hslToRgb(2.55f, hue, 0.53f), projectile.scale / 1.4f);
                }
                if (Main.rand.NextBool(5))
                {
                    Vector2 value42 = projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * (float)projectile.width;
                    int num847 = Dust.NewDust(vector80 + value42 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 5f);
                    Main.dust[num847].velocity *= 0.5f;
                    Main.dust[num847].velocity.Y = -Math.Abs(Main.dust[num847].velocity.Y);
                }
                DelegateMethods.v3_1 = color.ToVector3() * 0.3f;
                float value43 = 0.1f * (float)Math.Sin((double)(Main.GlobalTime * 20f));
                Vector2 size = new Vector2(projectile.velocity.Length() * projectile.localAI[1], (float)projectile.width * projectile.scale);
                float num848 = projectile.velocity.ToRotation();
                if (Main.netMode != NetmodeID.Server)
                {
                    ((WaterShaderData)Filters.Scene["WaterDistortion"].GetShader()).QueueRipple(projectile.position + new Vector2(size.X * 0.5f, 0f).RotatedBy((double)num848, default), new Color(0.5f, 0.1f * (float)Math.Sign(value43) + 0.5f, 0f, 1f) * Math.Abs(value43), size, RippleShape.Square, num848);
                }
                Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
                return;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D tex = Main.projectileTexture[projectile.type];
            float num228 = projectile.localAI[1];
            float prismHue = GetHue(projectile.ai[0]);
            Color value25 = Main.hslToRgb(2.55f, prismHue, 0.53f);
            value25.A = 0;
            Vector2 value26 = projectile.Center.Floor();
            value26 += projectile.velocity * projectile.scale * 10.5f;
            num228 -= projectile.scale * 14.5f * projectile.scale;
            Vector2 vector29 = new Vector2(projectile.scale);
            DelegateMethods.f_1 = 1f;
            DelegateMethods.c_1 = value25 * 0.75f * projectile.Opacity;
            Vector2 projPos = projectile.oldPos[0];
            projPos = new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Utils.DrawLaser(Main.spriteBatch, tex, value26 - Main.screenPosition, value26 + projectile.velocity * num228 - Main.screenPosition, vector29, new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));
            DelegateMethods.c_1 = new Color(255, Main.DiscoG, 53, 127) * 0.75f * projectile.Opacity;
            Utils.DrawLaser(Main.spriteBatch, tex, value26 - Main.screenPosition, value26 + projectile.velocity * num228 - Main.screenPosition, vector29 / 2f, new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Center, projectile.Center + unit * projectile.localAI[1], (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], 22f * projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }
    }
}
