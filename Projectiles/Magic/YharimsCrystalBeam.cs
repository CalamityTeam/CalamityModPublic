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
    public class YharimsCrystalBeam : ModProjectile
    {
        private const float PiBeamDivisor = MathHelper.Pi / YharimsCrystalPrism.NumBeams;

        private const float MaxDamageMultiplier = 3f;

        private const float BeamPosOffset = 16f;
        private const float MaxBeamScale = 1.8f;

        private const float MaxBeamLength = 2400f;
        private const float BeamTileCollisionWidth = 1f;
        private const float BeamHitboxCollisionWidth = 22f;
        private const int NumSamplePoints = 3;
        private const float BeamLengthChangeFactor = 0.75f;

        private const float VisualEffectThreshold = 0.1f;

        private const float OuterBeamOpacityMultiplier = 0.75f;
        private const float InnerBeamOpacityMultiplier = 0.1f;
        private const float BeamLightBrightness = 0.75f;

        private const float MainDustBeamEndOffset = 14.5f;
        private const float SidewaysDustBeamEndOffset = 4f;
        private const float BeamRenderTileOffset = 10.5f;
        private const float BeamLengthReductionFactor = 14.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharim's Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            // The beam itself still stops on tiles, but its invisible "source" projectile ignores them.
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Calamity().PierceResistHarshness = 0.06f;
            Projectile.Calamity().PierceResistCap = 0.4f;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(Projectile.localAI[1]);
        public override void ReceiveExtraAI(BinaryReader reader) => Projectile.localAI[1] = reader.ReadSingle();

        // projectile.ai[0] = 0 through N-1, chosen at spawn = Which beam ID this is (they all have different rotations and colors)
        // projectile.ai[1] = Index of the crystal projectile "hosting" this beam
        // projectile.localAI[1] = Length of the beam (dynamically calculated every frame)
        // The beam projectile also makes heavy use of its host crystal's charge level.
        public override void AI()
        {
            // If something has gone wrong with either the beam or the host crystal, destroy the beam.
            Projectile hostCrystal = Main.projectile[(int)Projectile.ai[1]];
            if (Projectile.type != ModContent.ProjectileType<YharimsCrystalBeam>() || !hostCrystal.active || hostCrystal.type != ModContent.ProjectileType<YharimsCrystalPrism>())
            {
                Projectile.Kill();
                return;
            }
            Vector2 hostCrystalDir = Vector2.Normalize(hostCrystal.velocity);
            float chargeRatio = MathHelper.Clamp(hostCrystal.ai[0] / YharimsCrystalPrism.MaxCharge, 0f, 1f);

            // Update the beam's damage based on the host crystal's current damage value, which accounts for Mana Sickness.
            // Yharim's Crystal smoothly scales up its damage instead of suddenly jumping to a higher damage output.
            Projectile.damage = (int)(hostCrystal.damage * GetDamageMultiplier(chargeRatio));

            // The beam cannot strike enemies until at a certain charge level.
            Projectile.friendly = hostCrystal.ai[0] > YharimsCrystalPrism.DamageStart;

            // This offset is used to make the beams orient differently.
            float beamIdOffset = (int)Projectile.ai[0] - YharimsCrystalPrism.NumBeams / 2f + 0.5f;
            float beamSpread;
            float spinRate;
            float beamStartSidewaysOffset;
            float beamStartForwardsOffset;

            // Variables scale smoothly while the crystal is charging up, but are fixed once it is at max charge.
            if (chargeRatio < 1f)
            {
                Projectile.scale = MathHelper.Lerp(0f, MaxBeamScale, chargeRatio);
                beamSpread = MathHelper.Lerp(1.22f, 0f, chargeRatio);
                beamStartSidewaysOffset = MathHelper.Lerp(20f, 6f, chargeRatio);
                beamStartForwardsOffset = MathHelper.Lerp(-17f, -13f, chargeRatio);

                // For the first 2/3 of charge time, the opacity scales up from 0% to 40%.
                // Spin rate increases slowly during this time.
                if (chargeRatio <= 0.66f)
                {
                    float phaseRatio = chargeRatio * 1.5f;
                    Projectile.Opacity = MathHelper.Lerp(0f, 0.4f, phaseRatio);
                    spinRate = MathHelper.Lerp(20f, 16f, phaseRatio);
                }

                // For the last 1/3 of charge time, the opacity scales up from 40% to 100%.
                // Spin rate increases dramatically during this time.
                else
                {
                    float phaseRatio = (chargeRatio - 0.66f) * 3f;
                    Projectile.Opacity = MathHelper.Lerp(0.4f, 1f, phaseRatio);
                    spinRate = MathHelper.Lerp(16f, 6f, phaseRatio);
                }
            }
            else
            {
                Projectile.scale = MaxBeamScale;
                Projectile.Opacity = 1f;
                beamSpread = 0f;
                spinRate = 6f;
                beamStartSidewaysOffset = 6f;
                beamStartForwardsOffset = -13f;
            }

            // The amount to which the angle changes reduces over time so that the beams look like they are focusing.
            float deviationAngle = (hostCrystal.ai[0] + beamIdOffset * spinRate) / (spinRate * YharimsCrystalPrism.NumBeams) * MathHelper.TwoPi;
            Vector2 unitRot = Vector2.UnitY.RotatedBy(deviationAngle);
            float sinusoidYOffset = unitRot.Y * PiBeamDivisor * beamSpread;
            float hostCrystalAngle = hostCrystal.velocity.ToRotation();
            Vector2 yVec = new Vector2(4f, beamStartSidewaysOffset);
            Vector2 beamSpanVector = (unitRot * yVec).RotatedBy(hostCrystalAngle);

            // Calculate the beam's emanating position. Start with the crystal center.
            Projectile.Center = hostCrystal.Center;
            // Add a fixed offset to align with the crystal's spritesheet (?)
            Projectile.position += hostCrystalDir * BeamPosOffset + new Vector2(0f, -hostCrystal.gfxOffY);
            // Add the forwards offset, measured in pixels.
            Projectile.position += hostCrystalDir * beamStartForwardsOffset;
            // Add the sideways offset vector, which is calculated for the current angle of the beam and scales with the beam's sideways offset.
            Projectile.position += beamSpanVector;

            // Set the beam's velocity to point towards its current spread direction and sanity check it. It should have magnitude 1.
            Projectile.velocity = hostCrystalDir.RotatedBy(sinusoidYOffset);
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // By default, the interpolation starts at the projectile's center.
            // If the host crystal is fully charged, the interpolation starts at the host crystal's center instead.
            // Overriding that, if the player shoves the crystal into or through a wall, the interpolation starts at the player's center.
            Vector2 samplingPoint = Projectile.Center;
            if(hostCrystal.ai[0] >= YharimsCrystalPrism.MaxCharge)
                samplingPoint = hostCrystal.Center;
            if (!Collision.CanHitLine(Main.player[Projectile.owner].Center, 0, 0, hostCrystal.Center, 0, 0))
                samplingPoint = Main.player[Projectile.owner].Center;

            // Perform a laser scan to calculate the correct length of the beam.
            // Alternatively, if the beam ignores tiles, just set it to be the max beam length with the following line.
            // projectile.localAI[1] = MaxBeamLength;
            float[] laserScanResults = new float[NumSamplePoints];
            Collision.LaserScan(samplingPoint, Projectile.velocity, BeamTileCollisionWidth * Projectile.scale, MaxBeamLength, laserScanResults);
            float avg = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
                avg += laserScanResults[i];
            avg /= NumSamplePoints;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], avg, BeamLengthChangeFactor);

            // X = beam length. Y = beam width.
            Vector2 beamDims = new Vector2(Projectile.velocity.Length() * Projectile.localAI[1], Projectile.width * Projectile.scale);

            // Only produce dust and cause water ripples if the beam is above a certain charge level.
            Color beamColor = GetBeamColor();
            if (chargeRatio >= VisualEffectThreshold)
            {
                ProduceBeamDust(beamColor);

                // If the game is rendering (i.e. isn't a dedicated server), make the beam disturb water.
                if (Main.netMode != NetmodeID.Server)
                {
                    WaterShaderData wsd = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();
                    // A universal time-based sinusoid which updates extremely rapidly. GlobalTime is 0 to 3600, measured in seconds.
                    float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
                    Vector2 ripplePos = Projectile.position + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(Projectile.rotation);
                    // WaveData is encoded as a Color. Not sure why, considering Vector3 exists.
                    Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
                    wsd.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation);
                }
            }

            // Make the beam cast light along its length. The brightness of the light scales with the charge.
            // v3_1 is an unnamed decompiled variable which is the color of the light cast by DelegateMethods.CastLight
            DelegateMethods.v3_1 = beamColor.ToVector3() * BeamLightBrightness * chargeRatio;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], beamDims.Y, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        // Uses a simple polynomial (x^3) to get sudden but smooth damage increase near the end of the charge-up period.
        private float GetDamageMultiplier(float chargeRatio)
        {
            float f = chargeRatio * chargeRatio * chargeRatio;
            return MathHelper.Lerp(1f, MaxDamageMultiplier, f);
        }

        // Determines whether the specified target hitbox is intersecting with the beam.
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host crystal), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
                return true;
            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], BeamHitboxCollisionWidth * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // If the beam doesn't have a defined direction, don't draw anything.
            if (Projectile.velocity == Vector2.Zero)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            float beamLength = Projectile.localAI[1];
            Vector2 centerFloored = Projectile.Center.Floor() + Projectile.velocity * Projectile.scale * BeamRenderTileOffset;
            Vector2 scaleVec = new Vector2(Projectile.scale);

            // Reduce the beam length proportional to its square area to reduce block penetration.
            beamLength -= BeamLengthReductionFactor * Projectile.scale * Projectile.scale;

            DelegateMethods.f_1 = 1f; // f_1 is an unnamed decompiled variable whose function is unknown. Leave it at 1.
            Vector2 beamStartPos = centerFloored - Main.screenPosition;
            Vector2 beamEndPos = beamStartPos + Projectile.velocity * beamLength;
            Utils.LaserLineFraming llf = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);

            // Draw the outer beam
            // c_1 is an unnamed decompiled variable which is the render color of the beam drawn by DelegateMethods.RainbowLaserDraw
            Color outerBeamColor = GetBeamColor();
            DelegateMethods.c_1 = outerBeamColor * OuterBeamOpacityMultiplier * Projectile.Opacity;
            Utils.DrawLaser(Main.spriteBatch, tex, beamStartPos, beamEndPos, scaleVec, llf);

            // Draw the inner beam (reduced size)
            scaleVec *= 0.5f;
            Color innerBeamColor = Color.White;
            DelegateMethods.c_1 = innerBeamColor * InnerBeamOpacityMultiplier * Projectile.Opacity;
            Utils.DrawLaser(Main.spriteBatch, tex, beamStartPos, beamEndPos, scaleVec, llf);
            return false;
        }

        private void ProduceBeamDust(Color beamColor)
        {
            // Create a few dust per frame a small distance from where the beam ends.
            Vector2 laserEndPos = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - MainDustBeamEndOffset * Projectile.scale);
            for (int i = 0; i < 2; ++i)
            {
                // 50% chance for the dust to come off on either side of the beam.
                float dustAngle = Projectile.rotation + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
                float dustStartDist = Main.rand.NextFloat(1f, 1.8f);
                Vector2 dustVel = dustAngle.ToRotationVector2() * dustStartDist;
                int d = Dust.NewDust(laserEndPos, 0, 0, 244, dustVel.X, dustVel.Y, 0, beamColor, 3.3f);
                Main.dust[d].color = beamColor;
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = 1.2f;

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
                Vector2 dustOffset = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * (Main.rand.NextFloat() - 0.5f) * Projectile.width;
                Vector2 dustPos = laserEndPos + dustOffset - Vector2.One * SidewaysDustBeamEndOffset;
                int dustID = 244;
                int d = Dust.NewDust(dustPos, 8, 8, dustID, 0f, 0f, 100, beamColor, 5f);
                Main.dust[d].velocity *= 0.5f;

                // Force the dust to always move downwards, never upwards.
                Main.dust[d].velocity.Y = -Math.Abs(Main.dust[d].velocity.Y);
            }
        }

        private Color GetBeamColor()
        {
            float customHue = GetHue(Projectile.ai[0]);
            float sat = 0.66f;
            Color c = Main.hslToRgb(customHue, sat, 0.53f);
            c.A = 64;
            return c;
        }

        // indexing = 0 through N-1 (it's beam ID, stored in projectile.ai[0])
        private float GetHue(float indexing)
        {
            string name = Main.player[Projectile.owner].name ?? "";
            if (Main.player[Projectile.owner].active)
            {
                switch (name)
                {
                    case "Fabsol":
                    case "Ziggums":
                        return 2f;
                    case "Poly":
                        return 0.83f;
                    case "Zach":
                        return 1.5f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.1f;
                    case "Grox the Great":
                        return 1.27f;
                    case "Jenosis":
                        return 0.65f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.1f;
                    case "DM DOKURO":
                        return 0f;
                    case "Uncle Danny":
                    case "Phoenix":
                        return 1.7f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.07f;
                    case "Minecat":
                        return 0.15f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.07f;
                    case "Khaelis":
                        return 1.15f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.18f;
                    case "Purple Necromancer":
                        return 1.7f + (float)Math.Cos(Main.time / 120.0 * Math.PI * 2.0) * 0.05f;
                    case "gamagamer64":
                        return 0.83f + (float)Math.Cos(Main.time / 120.0 * Math.PI * 2.0) * 0.03f;
                    case "Svante":
                        return 1.4f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.06f;
                    case "Puff":
                        return 0.31f + (float)Math.Cos(Main.time / 120.0 * Math.PI * 2.0) * 0.13f;
                    case "Leviathan":
                        return 1.9f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.1f;
                    case "Testdude":
                        return Main.rand.NextFloat();
                }
            }
            // Something in the range of red to yellow
            return (indexing / YharimsCrystalPrism.NumBeams) % 0.12f;
        }

        // Automatically iterates through every tile the laser is overlapping to cut grass at all those locations.
        public override void CutTiles()
        {
            // tilecut_0 is an unnamed decompiled variable which tells CutTiles how the tiles are being cut (in this case, via a projectile).
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PerLinePoint cut = new Utils.PerLinePoint(DelegateMethods.CutTiles);
            Vector2 beamStartPos = Projectile.Center;
            Vector2 beamEndPos = beamStartPos + Projectile.velocity * Projectile.localAI[1];
            Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
        }
    }
}
