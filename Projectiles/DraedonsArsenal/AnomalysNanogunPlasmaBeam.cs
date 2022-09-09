using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using CalamityMod.Particles;
using System;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class AnomalysNanogunPlasmaBeam : BaseLaserbeamProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/YharimsCrystalBeam";

        // All 3 point to the same so that BaseLaserbeamProjectile can still be used with an animated laser
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/AnomalysNanogunPlasmaBeam", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => LaserBeginTexture;
        public override Texture2D LaserEndTexture => LaserBeginTexture;

        public override float MaxScale => 1f;
        public override float Lifetime => 12f;
        public override float MaxLaserLength => 2200f;
        public override Color LaserOverlayColor => Color.White;
        public override Color LightCastColor => Color.OrangeRed;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.scale = MaxScale;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 12;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Main.projFrames[Projectile.type] = 18;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            base.AI();
        }

        public override void ExtraBehavior()
        {
            if (Projectile.timeLeft == 12)
            {
                Vector2 beamVector = Projectile.velocity;
                float beamLength = DetermineLaserLength_CollideWithTiles(12);

                //Rapid dust
                int dustCount = Main.rand.Next(10, 30);
                for (int i = 0; i < dustCount; i++)
                {
                    float dustProgressAlongBeam = beamLength * Main.rand.NextFloat(0f, 0.8f);
                    Vector2 dustPosition = Projectile.Center + dustProgressAlongBeam * beamVector + beamVector.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-6f, 6f) * Projectile.scale;

                    Dust dust = Dust.NewDustPerfect(dustPosition, 187, beamVector * Main.rand.NextFloat(5f, 26f), 0, Color.OrangeRed, 2.2f);
                    dust.noGravity = true;
                }

                // Energy rings around the laser
                int step = 10;
                while (step < LaserLength)
                {
                    Particle pulse = new DirectionalPulseRing(Projectile.Center + MathHelper.WrapAngle(Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * step, Vector2.Zero, Color.Red, new Vector2(0.5f, 1f), MathHelper.WrapAngle(Projectile.rotation + MathHelper.PiOver2), 0.1f, 1f, 12);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    step += 100;
                }
            }

            else
                Projectile.frameCounter++;
        }

        public override void DetermineScale() => Projectile.scale = 1f;

        public override float DetermineLaserLength() => DetermineLaserLength_CollideWithTiles(5);

        public override bool ShouldUpdatePosition() => false;

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, Projectile.width + 16, DelegateMethods.CutTiles);
        }

        // Animated laser
        public override bool PreDraw(ref Color lightColor)
        {
            int frame = Projectile.frameCounter / 2;
            DrawPlasmaBeam(Color.White, 1f, frame + 12, frame + 6, frame);
            return false;
        }

        private void DrawPlasmaBeam(Color beamColor, float scale, int startFrame = 0, int middleFrame = 0, int endFrame = 0)
        {
            Rectangle startFrameArea = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, startFrame);
            Rectangle middleFrameArea = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, middleFrame);
            Rectangle endFrameArea = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, endFrame);

            // Start texture drawing.
            Main.EntitySpriteDraw(LaserBeginTexture,
                             Projectile.Center - Main.screenPosition,
                             startFrameArea,
                             beamColor,
                             MathHelper.WrapAngle(Projectile.rotation + MathHelper.Pi),
                             startFrameArea.Size() / 2f,
                             scale,
                             SpriteEffects.None,
                             0);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength;
            laserBodyLength -= (startFrameArea.Height / 2 + endFrameArea.Height) * scale;
            Vector2 centerOnLaser = Projectile.Center;
            centerOnLaser += Projectile.velocity * scale * startFrameArea.Height / 2f;

            // Body drawing.
            if (laserBodyLength > 0f)
            {
                float laserOffset = middleFrameArea.Height * scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    Main.EntitySpriteDraw(LaserMiddleTexture,
                                     centerOnLaser - Main.screenPosition,
                                     middleFrameArea,
                                     beamColor,
                                     Projectile.rotation,
                                     LaserMiddleTexture.Width * 0.5f * Vector2.UnitX,
                                     scale,
                                     SpriteEffects.None,
                                     0);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += Projectile.velocity * laserOffset;
                }
            }

            // End texture drawing.
            if (Math.Abs(LaserLength - DetermineLaserLength()) < 30f)
            {
                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
                Main.EntitySpriteDraw(LaserEndTexture,
                                 laserEndCenter,
                                 endFrameArea,
                                 beamColor,
                                 MathHelper.WrapAngle(Projectile.rotation + MathHelper.Pi),
                                 endFrameArea.Size() / 2f,
                                 scale,
                                 SpriteEffects.None,
                                 0);
            }
        }
    }
}
