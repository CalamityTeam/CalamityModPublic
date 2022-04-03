using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ValkyrieRayBeam : ModProjectile
    {
        private const int Lifetime = 24;
        private const int BeamDustID = 73;

        private const float MaxBeamScale = 1.2f;

        private const float MaxBeamLength = 2400f;
        private const float BeamTileCollisionWidth = 1f;
        private const float BeamHitboxCollisionWidth = 15f;
        private const int NumSamplePoints = 3;
        private const float BeamLengthChangeFactor = 0.75f;

        private const float OuterBeamOpacityMultiplier = 0.82f;
        private const float InnerBeamOpacityMultiplier = 0.2f;
        private const float MaxBeamBrightness = 0.75f;

        private const float MainDustBeamEndOffset = 14.5f;
        private const float SidewaysDustBeamEndOffset = 4f;
        private const float BeamRenderTileOffset = 10.5f;
        private const float BeamLengthReductionFactor = 14.5f;

        private Vector2 beamVector = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valkyrie Ray");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            // The beam itself still stops on tiles, but its invisible "source" projectile ignores them.
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;

            // The beam lasts for only some frames and fades out over that time.
            Projectile.timeLeft = Lifetime;
        }

        // projectile.ai[0] = Length of the beam (dynamically recalculated every frame in case someone breaks or places some blocks)
        public override void AI()
        {
            // If the projectile for some reason isn't still a Valkyrie Ray, delete it immediately.
            if (Projectile.type != ModContent.ProjectileType<ValkyrieRayBeam>())
            {
                Projectile.Kill();
                return;
            }

            // On frame 1, set the beam vector and rotation, but set the real velocity to zero.
            if(Projectile.velocity != Vector2.Zero)
            {
                beamVector = Vector2.Normalize(Projectile.velocity);
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.velocity = Vector2.Zero;
            }

            // Reduce the "power" and thus scale of the projectile over its lifetime.
            float power = (float)Projectile.timeLeft / Lifetime;
            Projectile.scale = MaxBeamScale * power;

            // Perform a laser scan to calculate the correct length of the beam.
            float[] laserScanResults = new float[NumSamplePoints];

            // A minimum width is forced for the beam scan to prevent massive lag when fired into open areas.
            float scanWidth = Projectile.scale < 1f ? 1f : Projectile.scale;
            Collision.LaserScan(Projectile.Center, beamVector, BeamTileCollisionWidth * scanWidth, MaxBeamLength, laserScanResults);
            float avg = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
                avg += laserScanResults[i];
            avg /= NumSamplePoints;
            Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], avg, BeamLengthChangeFactor);

            // X = beam length. Y = beam width.
            Vector2 beamDims = new Vector2(beamVector.Length() * Projectile.ai[0], Projectile.width * Projectile.scale);

            Color beamColor = GetBeamColor();
            ProduceBeamDust(beamColor);

            // If the game is rendering (i.e. isn't a dedicated server), make the beam disturb water.
            if (Main.netMode != NetmodeID.Server)
            {
                WaterShaderData wsd = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();
                // A universal time-based sinusoid which updates extremely rapidly. GlobalTime is 0 to 3600, measured in seconds.
                float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTime * 20f);
                Vector2 ripplePos = Projectile.position + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(Projectile.rotation);
                // WaveData is encoded as a Color. Not sure why, considering Vector3 exists.
                Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
                wsd.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation);
            }

            // Make the beam cast light along its length.
            // v3_1 is an unnamed decompiled variable which is the color of the light cast by DelegateMethods.CastLight
            DelegateMethods.v3_1 = beamColor.ToVector3() * power * MaxBeamBrightness;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + beamVector * Projectile.ai[0], beamDims.Y, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        // Determines whether the specified target hitbox is intersecting with the beam.
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host crystal), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
                return true;
            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + beamVector * Projectile.ai[0], BeamHitboxCollisionWidth * Projectile.scale, ref _);
        }

        // Ensure that the hit direction is correct when hitting enemies.
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            hitDirection = (Projectile.Center.X < target.Center.X).ToDirectionInt();
        }

        private Color GetBeamColor()
        {
            Color c = ValkyrieRay.LightColor;
            c.A = 64;
            return c;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // If the beam doesn't have a defined projection vector or hasn't yet had its velocity set to zero, don't draw anything.
            if (beamVector == Vector2.Zero || Projectile.velocity != Vector2.Zero)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            float beamLength = Projectile.ai[0];
            Vector2 centerFloored = Projectile.Center.Floor() + beamVector * Projectile.scale * BeamRenderTileOffset;
            Vector2 scaleVec = new Vector2(Projectile.scale);

            // Reduce the beam length proportional to its square area to reduce block penetration.
            beamLength -= BeamLengthReductionFactor * Projectile.scale * Projectile.scale;

            DelegateMethods.f_1 = 1f; // f_1 is an unnamed decompiled variable whose function is unknown. Leave it at 1.
            Vector2 beamStartPos = centerFloored - Main.screenPosition;
            Vector2 beamEndPos = beamStartPos + beamVector * beamLength;
            Utils.LaserLineFraming llf = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);

            // Draw the outermost beam
            // c_1 is an unnamed decompiled variable which is the render color of the beam drawn by DelegateMethods.RainbowLaserDraw
            Color beamColor = GetBeamColor();
            DelegateMethods.c_1 = beamColor * OuterBeamOpacityMultiplier * Projectile.Opacity;
            Utils.DrawLaser(Main.spriteBatch, tex, beamStartPos, beamEndPos, scaleVec, llf);

            // Draw the inner beams, each with reduced size and whiter color
            for (int i = 0; i < 5; ++i)
            {
                beamColor = Color.Lerp(beamColor, Color.White, 0.4f);
                scaleVec *= 0.85f;
                DelegateMethods.c_1 = beamColor * InnerBeamOpacityMultiplier * Projectile.Opacity;
                Utils.DrawLaser(Main.spriteBatch, tex, beamStartPos, beamEndPos, scaleVec, llf);
            }
            return false;
        }

        private void ProduceBeamDust(Color beamColor)
        {
            // Create a few dust per frame a small distance from where the beam ends.
            Vector2 laserEndPos = Projectile.Center + beamVector * (Projectile.ai[0] - MainDustBeamEndOffset * Projectile.scale);
            for (int i = 0; i < 2; ++i)
            {
                // 50% chance for the dust to come off on either side of the beam.
                float dustAngle = Projectile.rotation + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
                float dustStartDist = Main.rand.NextFloat(1f, 1.8f);
                Vector2 dustVel = dustAngle.ToRotationVector2() * dustStartDist;
                int d = Dust.NewDust(laserEndPos, 0, 0, BeamDustID, dustVel.X, dustVel.Y, 0, beamColor);
                Main.dust[d].color = beamColor;
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = 0.7f;

                // Scale up dust with the projectile if it's large.
                if (Projectile.scale > 1f)
                {
                    Main.dust[d].velocity *= Projectile.scale;
                    Main.dust[d].scale *= Projectile.scale;
                }

                // If the beam isn't at max scale, then make additional smaller dust.
                if (Projectile.scale != MaxBeamScale)
                {
                    Dust smallDust = Dust.CloneDust(d);
                    smallDust.scale /= 2f;
                }
            }

            // Low chance every frame to spawn a large "directly sideways" dust which doesn't move.
            if (Main.rand.NextBool(5))
            {
                // Velocity, flipped sideways, times -50% to 50% of beam width.
                Vector2 dustOffset = beamVector.RotatedBy(MathHelper.PiOver2) * (Main.rand.NextFloat() - 0.5f) * Projectile.width;
                Vector2 dustPos = laserEndPos + dustOffset - Vector2.One * SidewaysDustBeamEndOffset;
                int d = Dust.NewDust(dustPos, 8, 8, BeamDustID, 0f, 0f, 100, beamColor, 1.2f);
                Main.dust[d].velocity *= 0.5f;

                // Force the dust to always move downwards, never upwards.
                Main.dust[d].velocity.Y = -Math.Abs(Main.dust[d].velocity.Y);
            }
        }

        // Automatically iterates through every tile the laser is overlapping to cut grass at all those locations.
        public override void CutTiles()
        {
            // tilecut_0 is an unnamed decompiled variable which tells CutTiles how the tiles are being cut (in this case, via a projectile).
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PerLinePoint cut = new Utils.PerLinePoint(DelegateMethods.CutTiles);
            Vector2 beamStartPos = Projectile.Center;
            Vector2 beamEndPos = beamStartPos + beamVector * Projectile.ai[0];
            Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
        }
    }
}
