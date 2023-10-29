using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class DarkSparkBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/Magic/YharimsCrystalBeam";

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
            Vector2? chargeUpCenter = null;

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            if (Projectile.type != ModContent.ProjectileType<DarkSparkBeam>() || !Main.projectile[(int)Projectile.ai[1]].active || Main.projectile[(int)Projectile.ai[1]].type != ModContent.ProjectileType<DarkSparkPrism>())
            {
                Projectile.Kill();
                return;
            }

            float laserPosition = (int)Projectile.ai[0] - 2.5f;
            Vector2 aimDirection = Vector2.Normalize(Main.projectile[(int)Projectile.ai[1]].velocity);
            Projectile projectile2 = Main.projectile[(int)Projectile.ai[1]];
            float laserNormalize;
            Vector2 projDirection = Vector2.Zero;
            float laserTimer;
            float y;
            float laserRotationSpeed;
            float scaleFactor6;
            Color color = new Color(1, 1, 1, 127);
            Projectile.Opacity = 1f;

            if (projectile2.ai[0] < 720f)
            {
                laserTimer = projectile2.ai[0] / 1440f;
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

                laserTimer = 0.5f;
                laserRotationSpeed = 10.875f;
                y = 13f;
                scaleFactor6 = -7f;
            }

            float laserDirection = (projectile2.ai[0] + laserPosition * laserRotationSpeed) / (laserRotationSpeed * 6f) * MathHelper.TwoPi;
            laserNormalize = Vector2.UnitY.RotatedBy(laserDirection).Y * (MathHelper.Pi / 6) * laserTimer * 0.33f;
            projDirection = (Vector2.UnitY.RotatedBy(laserDirection) * new Vector2(4f, y)).RotatedBy(projectile2.velocity.ToRotation());
            Projectile.position = projectile2.Center + aimDirection * 16f - Projectile.Size / 2f + new Vector2(0f, -Main.projectile[(int)Projectile.ai[1]].gfxOffY);
            Projectile.position += projectile2.velocity.ToRotation().ToRotationVector2() * scaleFactor6;
            Projectile.position += projDirection;
            Projectile.velocity = Vector2.Normalize(projectile2.velocity).RotatedBy(laserNormalize);
            Projectile.scale = 1.5f * (1.5f - laserTimer);

            // Takes 360 frames to reach normal damage
            float amount = projectile2.ai[0] / 1200f;
            if (amount > 1f)
                amount = 1f;
            Projectile.damage = (int)(projectile2.damage * MathHelper.Lerp(0.25f, 2.2f, amount));

            if (projectile2.ai[0] >= 720f)
                chargeUpCenter = new Vector2?(projectile2.Center);

            if (!Collision.CanHitLine(Main.player[Projectile.owner].Center, 0, 0, projectile2.Center, 0, 0))
                chargeUpCenter = new Vector2?(Main.player[Projectile.owner].Center);

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            float laserRotate = Projectile.velocity.ToRotation();
            Projectile.rotation = laserRotate - MathHelper.PiOver2;
            Projectile.velocity = laserRotate.ToRotationVector2();

            Vector2 samplingPoint = Projectile.Center;
            if (chargeUpCenter.HasValue)
                samplingPoint = chargeUpCenter.Value;

            float[] array3 = new float[2];
            Collision.LaserScan(samplingPoint, Projectile.velocity, 0f * Projectile.scale, 2400f, array3);
            float beamSeparation = 0f;
            for (int i = 0; i < array3.Length; i++)
                beamSeparation += array3[i];
            beamSeparation /= 2f;

            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], beamSeparation, 0.75f);
            if (Math.Abs(Projectile.localAI[1] - beamSeparation) < 100f && Projectile.scale > 0.15f)
            {
                Vector2 dustSpawn = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14.5f * Projectile.scale);
                for (int j = 0; j < 2; j++)
                {
                    float dustRotate = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                    float dustRandom = (float)Main.rand.NextDouble() * 0.8f + 1f;
                    Vector2 randomRotate = new Vector2((float)Math.Cos(dustRotate) * dustRandom, (float)Math.Sin(dustRotate) * dustRandom);
                    int rainbowDust = Dust.NewDust(dustSpawn, 0, 0, 267, randomRotate.X, randomRotate.Y, 0, color, 3.3f);
                    Main.dust[rainbowDust].color = color;
                    Main.dust[rainbowDust].scale = 1.2f;
                    if (Projectile.scale > 1f)
                    {
                        Main.dust[rainbowDust].velocity *= Projectile.scale;
                        Main.dust[rainbowDust].scale *= Projectile.scale;
                    }
                    Main.dust[rainbowDust].noGravity = true;
                    if (Projectile.scale != 1.4f)
                    {
                        Dust rainbowClone = Dust.CloneDust(rainbowDust);
                        rainbowClone.color = color;
                        rainbowClone.scale /= 2f;
                        rainbowClone.noGravity = true;
                    }
                    Main.dust[rainbowDust].color = color;
                }

                if (Main.rand.NextBool(5))
                {
                    Vector2 extraDustSpawn = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                    int extraRainbows = Dust.NewDust(dustSpawn + extraDustSpawn - Vector2.One * 4f, 8, 8, 267, 0f, 0f, 100, color, 5f);
                    Main.dust[extraRainbows].velocity *= 0.5f;
                    Main.dust[extraRainbows].noGravity = true;
                    Main.dust[extraRainbows].velocity.Y = -Math.Abs(Main.dust[extraRainbows].velocity.Y);
                }

                DelegateMethods.v3_1 = color.ToVector3() * 0.3f;
                Vector2 size = new Vector2(Projectile.velocity.Length() * Projectile.localAI[1], Projectile.width * Projectile.scale);
                float shaderLength = Projectile.velocity.ToRotation();
                if (Main.netMode != NetmodeID.Server)
                    ((WaterShaderData)Filters.Scene["WaterDistortion"].GetShader()).QueueRipple(Projectile.position + new Vector2(size.X * 0.5f, 0f).RotatedBy(shaderLength), color, size, RippleShape.Square, shaderLength);

                Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            float drawArea = Projectile.localAI[1];
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
            Vector2 drawStart = Projectile.Center.Floor();
            drawStart += Projectile.velocity * Projectile.scale * 10.5f;
            drawArea -= Projectile.scale * 14.5f * Projectile.scale;
            Vector2 drawScale = new Vector2(Projectile.scale);
            DelegateMethods.f_1 = 1f;
            DelegateMethods.c_1 = value25 * 0.75f * Projectile.Opacity;
            Vector2 projPos = Projectile.oldPos[0];
            projPos = new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Utils.DrawLaser(Main.spriteBatch, tex, drawStart - Main.screenPosition, drawStart + Projectile.velocity * drawArea - Main.screenPosition, drawScale, new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));
            DelegateMethods.c_1 = color * 0.75f * Projectile.Opacity;
            Utils.DrawLaser(Main.spriteBatch, tex, drawStart - Main.screenPosition, drawStart + Projectile.velocity * drawArea - Main.screenPosition, drawScale / 2f, new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw));
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

            float useless = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref useless))
                return true;

            return false;
        }
    }
}
